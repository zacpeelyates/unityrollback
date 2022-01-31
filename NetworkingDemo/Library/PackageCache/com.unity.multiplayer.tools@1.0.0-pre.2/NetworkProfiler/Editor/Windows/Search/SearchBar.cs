using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class SearchBar : VisualElement
    {
        static class VisualElementNames
        {
            internal const string ToolbarSearchField = "ToolbarSearchField";
        }

        static class VisualTreeAssetPaths
        {
            internal const string Search =
                "Packages/com.unity.multiplayer.tools/NetworkProfiler/Editor/Windows/Search/search.uxml";
        }

        readonly ToolbarSearchField m_ToolbarSearchField;
        List<IRowData> m_Entries = new List<IRowData>();
        static SearchListFilter Filter => Filters.PartialMatchGameObjectFilter;

        internal event Action<IReadOnlyCollection<IRowData>> OnSearchResultsChanged;
        internal event Action OnSearchStringCleared;

        public SearchBar()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreeAssetPaths.Search);
            var root = tree.CloneTree();

            m_ToolbarSearchField = root.Q<ToolbarSearchField>(VisualElementNames.ToolbarSearchField);
            m_ToolbarSearchField.value = DetailsViewPersistentState.SearchBarString;
            m_ToolbarSearchField.RegisterValueChangedCallback(OnSearchFieldChanged);

            tooltip = Tooltips.SearchBar;

            Add(root);
        }

        void OnSearchFieldChanged(ChangeEvent<string> searchString)
        {
            DetailsViewPersistentState.SearchBarString = searchString.newValue;
            RefreshSearchResults();
        }

        void RefreshSearchResults()
        {
            var searchString = m_ToolbarSearchField.value;
            var isSearching = !string.IsNullOrWhiteSpace(searchString);
            if (isSearching)
            {
                var entries = Filter.Invoke(m_Entries, searchString);
                OnSearchResultsChanged?.Invoke(entries);
                return;
            }
            OnSearchStringCleared?.Invoke();
        }

        internal void SetEntries(List<IRowData> searchListEntries)
        {
            m_Entries = searchListEntries;
            RefreshSearchResults();
        }

        internal void SetEntries(TreeModel treeModel)
        {
            SetEntries(TreeModelUtility.FlattenTree(treeModel));
        }
    }
}