using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Multiplayer.Tools.NetworkProfiler.Editor;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    class SearchTest
    {
        [Test]
        // Simple search filters
        [TestCase("", 23)]
        [TestCase("bu", 5)]
        [TestCase("bullet", 5)]
        [TestCase("bullet(", 4)]
        [TestCase("bullet(      ", 4)]
        [TestCase("    bullet(", 4)]
        [TestCase("    bullet(   ", 4)]
        [TestCase(" \t bullet(  \t ", 4)]
        [TestCase(" \t bullet   (  \t ", 0)]
        [TestCase("rp", 4)]
        [TestCase("mask", 0)]
        [TestCase("%^", 0)]
        [TestCase("ñ", 0)]
        [TestCase("网络", 0)]

        // Direction filters
        [TestCase("dir:in", 18)]
        [TestCase("dir:out", 14)]
        [TestCase("dir: out", 14)]
        [TestCase("dir :out", 14)]
        [TestCase("dir : out", 14)]

        // Multiple direction filters
        [TestCase("dir:in dir:out", 10)]

        // Direction and simple search filters
        [TestCase("dir:in bullet", 2)]
        [TestCase("dir:out bullet", 3)]
        [TestCase("bullet dir:out", 3)]
        [TestCase("dir:out rpc", 1)]
        [TestCase("dir:out rpc", 1)]

        // Type filters
        [TestCase("t:gameob", 5)]
        [TestCase("t:rp", 4)]
        [TestCase("t:pcevent", 4)]

        // Direction and type filters
        [TestCase("dir:in t:rpc", 3)]
        [TestCase("dir:in t:game", 2)]

        // Byte count filters
        [TestCase("b<1", 1)]
        [TestCase("b<2", 2)]
        [TestCase("b<3", 2)]
        [TestCase("b<4", 3)]
        [TestCase("b<5", 8)]
        [TestCase("b<8", 8)]
        [TestCase("b<9", 9)]
        [TestCase("b<12", 9)]
        [TestCase("b<=12", 10)]

        [TestCase("b>10", 14)]
        [TestCase("b>=8", 15)]

        [TestCase("b==12", 1)]
        [TestCase("b=12", 1)]
        [TestCase("b = 37", 12)]
        [TestCase("b != 37", 11)]

        // Multiple byte count filters
        [TestCase("b<5 b>10", 0)]
        [TestCase("b<=5 b>=10", 0)]
        [TestCase("b>5 b<10", 1)]
        [TestCase("b>5 b<50", 14)]
        [TestCase("b>5 b<50 b!=25", 14)]
        [TestCase("b>5 b<50 b!=37", 2)]

        // Multiple byte count filters of the same type
        [TestCase("b==4 b==37", 0)] // Not possible to equal to different things
        [TestCase("b!=4 b!=37", 6)] // Possible to not equal different things
        [TestCase("b!=4 b!=37 b<100", 5)]
        [TestCase("b>0 b>4 b<100", 14)] // Bit redundant, but possible input
        [TestCase("b<=32 b>0 b<=11", 8)] // Bit redundant, but possible input

        // Test cases with multiple simple search strings split by a command,
        // against "ready", "player 1", and "ready player 1"
        [TestCase("ready dir:in", 2)]
        [TestCase("dir:in player 1", 2)]
        [TestCase("player dir:in 1", 2)]
        [TestCase("ready dir:in player 1", 1)]
        [TestCase("steady dir:in player 1", 0)]
        [TestCase("ready dir:in mayor 1", 0)]

        // Test cases against "door:1"
        [TestCase("door", 1)]
        [TestCase("r:1", 1)]
        [TestCase("door:1", 1)]
        [TestCase("door :1", 0)]

        // Test cases against "row<statues>" and "bomb<7>"
        [TestCase("<", 2)]

        // Test cases against "row<statues>"
        [TestCase("row", 1)]
        [TestCase("w<", 1)]
        [TestCase("w<s", 1)]
        [TestCase("w<statues", 1)]
        [TestCase("w<statues>", 1)]
        [TestCase("row<statues>", 1)]
        [TestCase("row dir:in b>34 <statues>", 1)]
        [TestCase("statues> dir:in b>34 row<", 1)]
        [TestCase("statues> dir:in row< b>34", 1)]
        [TestCase("ro<stat", 0)]
        [TestCase("row<s>", 0)]
        [TestCase("row <s", 0)]

        // Test cases against "b>=c >= d"
        [TestCase("b>", 1)]
        [TestCase("=c", 1)]
        [TestCase(">=c", 1)]
        [TestCase(">= d", 1)]
        [TestCase("c >= d", 1)]
        [TestCase("b>=c >= d", 1)]

        // Test cases against "bomb<7>"
        [TestCase("bomb", 1)]
        [TestCase("<7", 1)]
        [TestCase("<7>", 1)]
        [TestCase("bomb<", 1)]
        [TestCase("bomb<7", 1)]
        [TestCase("bomb<7>", 1)]
        [TestCase("mb<7>", 1)]
        [TestCase("mb <7>", 0)]
        [TestCase("b<7", 8)] // Should be interpreted as a command, not a substring search
        [TestCase("b<7>", 0)] // Should be interpreted as a command, followed by a substring search

        // Test cases against "dir:on", "dir:initial", "dir : outer"
        [TestCase("dir", 3)]
        [TestCase("ir:", 2)]
        [TestCase("dir:", 2)]

        // Test cases against "dir:on"
        [TestCase("r:o", 1)]
        [TestCase("dir:o", 1)]
        [TestCase("dir:on", 1)]
        [TestCase("dir: on", 0)]
        [TestCase("dir:un", 0)]

        // Test cases against "dir:initial"
        [TestCase("r:i", 1)]
        [TestCase("dir:i", 1)]
        [TestCase("dir:init", 1)]
        [TestCase("dir:initial", 1)]
        [TestCase("dir: init", 0)]
        [TestCase("dir:int", 0)]

        // Test cases against "dir : outer"
        [TestCase("r :", 1)]
        [TestCase("r : o", 1)]
        [TestCase("dir :", 1)]
        [TestCase("dir : ou", 1)]
        [TestCase("out", 1)]
        [TestCase(": out", 1)]
        [TestCase(": outer", 1)]
        [TestCase("dir : outer", 1)]

        // Test cases against "ambient:sound"
        [TestCase("t:sound", 0)] // Interpreted as a command; there are no test entries with type sound
        [TestCase("nt:s", 1)]
        [TestCase("nt:sound", 1)]
        [TestCase("ambient:sound", 1)]

        public void Filters_PartialMatchFilterResults_Count(string search, int resultCount)
        {
            var list = new List<TestRowData>
            {
                // Game objects
                new TestRowData("Bullet"        , "gameobject", received: 1),
                new TestRowData("Bullet(1)"     , "gameobject", received: 4),
                new TestRowData("Bullet(2)"     , "gameobject", sent: 3),
                new TestRowData("Bullet(3)"     , "gameobject", sent: 8),
                new TestRowData("Bullet(4)"     , "gameobject", sent: 12),

                // RPC events
                new TestRowData("RpcEvent.Shoot", "rpcevent"  , sent: 4),
                new TestRowData("RpcEvent.Shoot", "rpcevent"  , received: 4),
                new TestRowData("RpcEvent.Shoot", "rpcevent"  , received: 4),
                new TestRowData("RpcEvent.Shoot", "rpcevent"  , received: 4),

                // Tricky test data (many include operators, keywords, or full search commands as substrings)
                new TestRowData("ready"         , "tricky"    , received: 37),
                new TestRowData("player 1"      , "tricky"    , received: 37),
                new TestRowData("ready player 1", "tricky"    , received: 37),
                new TestRowData("door:1"        , "tricky"    , sent: 30, received: 7),
                new TestRowData("row<statues>"  , "tricky"    , sent: 30, received: 7),
                new TestRowData("b>=c >= d"     , "tricky"    , sent: 30, received: 7),
                new TestRowData("bomb<7>"       , "tricky"    , sent: 30, received: 7),
                new TestRowData("9>b"           , "tricky"    , sent: 30, received: 7),
                new TestRowData("dir:on"        , "tricky"    , sent: 30, received: 7),
                new TestRowData("dir:initial"   , "tricky"    , sent: 30, received: 7),
                new TestRowData("dir : outer"   , "tricky"    , sent: 30, received: 7),
                new TestRowData("ambient:sound" , "tricky"    , sent: 30, received: 7),

                // Garbage/empty data
                new TestRowData(""              , ""          ),

                // Divided by two so that the total is (long.MaxValue - 1), rather than zero like the entry above
                new TestRowData("T44w1Ev"       , "jeRN3zD"   , sent: long.MaxValue / 2, received: long.MaxValue / 2),
            };
            var result = Filters.PartialMatchGameObjectFilter(list, search);

            Assert.AreEqual(resultCount, result.Count);
        }
    }

    /// Unit tests of internal filter methods as opposed to the main API
    internal class InternalFilterTests
    {
        [Test]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(4, 8)]
        [TestCase(5, 8)]
        [TestCase(6, 8)]
        [TestCase(7, 8)]
        [TestCase(8, 8)]
        [TestCase(9, -1)]
        [TestCase(10, -1)]
        public void FindNextNonWhitespaceToken(int startIndex, int resultIndex)
        {
            var tokens = new List<string>
            {
                "",                         //  0
                "a alphas",                 //  1
                "   ",                      //  2
                "   b numbers 0747 \n",     //  3
                "\t",                       //  4
                " \n \t  \t ",              //  5
                "",                         //  6
                " ",                        //  7
                "\t c symbols %'`?/(_)][]", //  8
                "",                         //  9
                " \n",                      // 10
            };
            var result = ParsedFilters.FindNextNonWhitespaceToken(tokens, startIndex);
            Assert.AreEqual(resultIndex, result);
        }

        [Test]
        [TestCase(0, 3)]
        [TestCase(1, null)]
        [TestCase(2, null)]
        [TestCase(3, null)]
        [TestCase(4, null)]
        [TestCase(5, null)]
        [TestCase(6, null)]
        [TestCase(7, null)]
        [TestCase(8, 11)]
        [TestCase(9, null)]
        [TestCase(10, null)]
        [TestCase(11, null)]
        [TestCase(12, 14)]
        [TestCase(13, null)]
        [TestCase(14, null)]
        public void TryFindPotentialCommand(int index, int? lastIndex)
        {
            var tokens = new List<string>
            {
                "dir",       //  0
                ":",         //  1
                " ",         //  2
                "in",        //  3
                " \n \n \t", //  4
                " ",         //  5
                "snail!",    //  6
                " ",         //  7
                "b",         //  8
                " ",         //  9
                "<=",        // 10
                "7",         // 11
                "t",         // 12
                ":",         // 13
                "rpc",       // 14
            };

            ParsedFilters.PotentialCommandAndLastTokenIndex? potentialCommand =
                ParsedFilters.TryFindPotentialCommand(tokens, index);

            Assert.AreEqual(potentialCommand?.lastTokenIndex, lastIndex);
        }
    }
}
