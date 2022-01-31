using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal delegate List<IRowData> SearchListFilter(
        IEnumerable<IRowData> searchListEntries,
        string queryString);

    internal static class Filters
    {
        public static List<IRowData> PartialMatchGameObjectFilter(
            IEnumerable<IRowData> searchListEntries,
            string queryString)
        {
            var parsedFilters = new ParsedFilters(queryString);
            return searchListEntries.Where(parsedFilters.Match).ToList();
        }
    }

    /// A set of filters parsed from a query string.
    /// Each type of filter corresponds to a field of this class.
    internal class ParsedFilters
    {
        delegate bool Filter(IRowData searchEntry);

        /// Simple search strings that are used to filter on the entry's name.
        /// We may encounter multiple simple search strings separated by commands:
        /// - .e.g. "ready dir:in player one"
        readonly List<string> m_SimpleSearchStringsLower = new List<string>();

        /// Arbitrary list of filter delegates that an are run against an Entry in Match
        readonly List<Filter> m_Filters = new List<Filter>();

        /// Returns true if the label matches all filters
        public bool Match(IRowData row)
        {
            var objectNameLower = row.Name.ToLower();
            return m_Filters.All(filter => filter(row))
                && m_SimpleSearchStringsLower.All(searchString => objectNameLower.Contains(searchString));
        }

        delegate Filter ArgumentParser(string argument);
        class FilterType
        {
            public string keyword;
            public List<string> operators;
            public ArgumentParser argumentParser;
            public FilterType(
                string keyword,
                string @operator,
                ArgumentParser argumentParser,
                string additionalOperator = null
                )
            {
                this.keyword = keyword;

                this.operators = new List<string>{@operator};
                if (additionalOperator != null)
                {
                    this.operators.Add(additionalOperator);
                }

                this.argumentParser = argumentParser;
            }
        }

        /// List of all filter commands, including their keyword, operator(s), parsing logic, and filtering logic
        static readonly ReadOnlyCollection<FilterType> k_FilterTypes;

        /// A list of all keywords, used during parsing
        static readonly ReadOnlyCollection<string> k_Keywords;

        /// A list of all operators, used during parsing, and to split the input query strings using regex
        /// Regex precedence is left to right, so longer operators should appear first, so that there's
        /// a chance to match longer operators before matching their shorter substrings
        /// (e.g. to match >= before matching > or =)
        static readonly ReadOnlyCollection<string> k_Operators;

        /// A map from operator+keyword to an argument parser used during parsing
        static readonly Dictionary<string, ArgumentParser> k_ParseTable;

        static ParsedFilters()
        {
            k_FilterTypes = new ReadOnlyCollection<FilterType>(new []
            {
                new FilterType("dir", ":", argument =>
                {
                    return argument switch
                    {
                        "in" => row => row.Bytes.Received > 0,
                        "out" => row => row.Bytes.Sent > 0,
                        _ => null
                    };
                }),
                new FilterType("t", ":", argument =>
                {
                    return row => row.TypeName.Contains(argument);
                }),
                new FilterType("b", "<", argument =>
                {
                    return long.TryParse(argument, out var value)
                        ? row => row.Bytes.Total < value
                        : (Filter)null;
                }),
                new FilterType("b", ">", argument =>
                {
                    return long.TryParse(argument, out var value)
                        ? row => row.Bytes.Total > value
                        : (Filter)null;
                }),
                new FilterType("b", "<=", argument =>
                {
                    return long.TryParse(argument, out var value)
                        ? row => row.Bytes.Total <= value
                        : (Filter)null;
                }),
                new FilterType("b", ">=", argument =>
                {
                    return long.TryParse(argument, out var value)
                        ? row => row.Bytes.Total >= value
                        : (Filter)null;
                }),
                new FilterType("b", "==", additionalOperator: "=", argumentParser: argument =>
                {
                    return long.TryParse(argument, out var value)
                        ? row => row.Bytes.Total == value
                        : (Filter)null;
                }),
                new FilterType("b", "!=", argument =>
                {
                    return long.TryParse(argument, out var value)
                        ? row => row.Bytes.Total != value
                        : (Filter)null;
                }),
            });

            k_Keywords = new ReadOnlyCollection<string>(
                k_FilterTypes
                    .Select(t => t.keyword)
                    .Distinct()
                    .ToList());

            k_Operators = new ReadOnlyCollection<string>(
                k_FilterTypes
                    .SelectMany(t => t.operators)
                    .Distinct()
                    .OrderByDescending(s => s.Length)
                    .ToList());

            k_ParseTable =
                k_FilterTypes
                    .SelectMany(t =>
                        t.operators.Select(@operator =>
                            new Tuple<string, ArgumentParser>(t.keyword + @operator, t.argumentParser)))
                    .ToDictionary(t => t.Item1, t => t.Item2);
        }

        /// All commands are composed of a keyword, an operator, and an argument, so any potential command
        /// must have all three
        internal readonly struct PotentialCommand
        {
            public string keyword { get; }
            public string @operator { get; }
            public string argument { get; }
            public PotentialCommand(string keyword, string @operator, string argument)
            {
                this.keyword = keyword;
                this.@operator = @operator;
                this.argument = argument;
            }
        }

        /// A potential command and the index of its last token
        internal readonly struct PotentialCommandAndLastTokenIndex
        {
            public PotentialCommand potentialCommand { get; }
            public int lastTokenIndex { get; }
            public PotentialCommandAndLastTokenIndex(PotentialCommand potentialCommand, int lastTokenIndex)
            {
                this.potentialCommand = potentialCommand;
                this.lastTokenIndex = lastTokenIndex;
            }
        }

        static bool IsNotWhiteSpace(string s) => !string.IsNullOrWhiteSpace(s);
        static bool IsNonEmpty(string s) => !string.IsNullOrEmpty(s);

        public ParsedFilters(string queryString)
        {
            var operatorAlternatives = string.Join("|", k_Operators);
            var operatorOrWhitespace = $"({operatorAlternatives}|\\s+)";

            // Tokens are substrings separated by whitespace or operators
            // The Regex.Split function inserts empty strings between adjacent matches, so filter those out
            var tokens = Regex.Split(queryString, operatorOrWhitespace).Where(IsNonEmpty).ToList();

            var currentSimpleSearchString = "";
            for (var i = 0; i < tokens.Count; ++i)
            {
                var potentialCommandAndLastIndex = TryFindPotentialCommand(tokens, i);
                if (!potentialCommandAndLastIndex.HasValue)
                {
                    currentSimpleSearchString += tokens[i];
                    continue;
                }
                if (!TryParsePotentialCommand(potentialCommandAndLastIndex.Value.potentialCommand))
                {
                    currentSimpleSearchString += tokens[i];
                    continue;
                }

                // We've found a valid command with a keyword, operator, and argument: advance the input stream and
                // add a new simple search string (if needed) for the next iteration of the loop
                i = potentialCommandAndLastIndex.Value.lastTokenIndex;
                if (IsNotWhiteSpace(currentSimpleSearchString))
                {
                    m_SimpleSearchStringsLower.Add(currentSimpleSearchString.Trim().ToLower());
                    currentSimpleSearchString = "";
                }
            }
            if (IsNotWhiteSpace(currentSimpleSearchString))
            {
                m_SimpleSearchStringsLower.Add(currentSimpleSearchString.Trim().ToLower());
            }
        }

        internal static int FindNextNonWhitespaceToken(List<string> tokens, int startIndex)
        {
            // List.FindNext throws an exception if start + count >= list.count,
            // so the Math.min call makes this safe
            var remaining = tokens.Count - startIndex;
            return tokens.FindIndex(startIndex, remaining, IsNotWhiteSpace);
        }

        internal static PotentialCommandAndLastTokenIndex? TryFindPotentialCommand(List<string> tokens, int index)
        {
            var keyword = tokens[index];
            if (!k_Keywords.Contains(keyword))
            {
                return null;
            }

            // The first non-whitespace string after the keyword is our prospective operator for a command,
            // such as `<`, `>=`, or `:`
            var opIndex = FindNextNonWhitespaceToken(tokens, index + 1);
            if (opIndex == -1)
            {
                return null;
            }
            var opString = tokens[opIndex];
            if (!k_Operators.Contains(opString))
            {
                return null;
            }

            // The non-whitespace string after the operator is our prospective right-hand operand
            var argumentIndex = FindNextNonWhitespaceToken(tokens, opIndex + 1);
            if (argumentIndex == -1)
            {
                return null;
            }
            var argument = tokens[argumentIndex];

            return new PotentialCommandAndLastTokenIndex(
                new PotentialCommand(keyword, opString, argument),
                argumentIndex);
        }

        bool TryParsePotentialCommand(PotentialCommand command)
        {
            var searchCommand = command.keyword + command.@operator;
            if (k_ParseTable.TryGetValue(searchCommand, out ArgumentParser argumentParser))
            {
                var filter = argumentParser(command.argument);
                if (filter != null)
                {
                    m_Filters.Add(filter);
                    return true;
                }
            }
            return false;
        }
    }
}
