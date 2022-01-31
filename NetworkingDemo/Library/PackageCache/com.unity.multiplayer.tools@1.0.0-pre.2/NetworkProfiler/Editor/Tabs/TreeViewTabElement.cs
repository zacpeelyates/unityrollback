using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class TreeViewTabElement : TabElement
    {
        ColumnBarNetwork m_ColumnBarNetwork;
        TreeModel m_TreeModel;
        TreeViewNetwork.DisplayType m_DisplayType;
        ListViewContainer m_FilteredResultsArea;
        SearchBar m_SearchBar;
        bool m_ShowFiltered;
        bool m_ShowStandard;
        TreeViewNetwork m_TreeView;
        VisualElement m_TreeViewArea;

        public void SetTreeViewDisplayType(TreeViewNetwork.DisplayType displayType)
        {
            m_DisplayType = displayType;
        }

        public override void UpdateMetrics(MetricCollection metricCollection)
        {
            m_TreeModel = ConstructTreeModel(metricCollection, m_DisplayType);
        }

        static TreeModel ConstructTreeModel(MetricCollection metricCollection, TreeViewNetwork.DisplayType displayType)
        {
            return displayType switch
            {
                TreeViewNetwork.DisplayType.Messages => TreeModelUtility.CreateMessagesTreeStructure(metricCollection),
                TreeViewNetwork.DisplayType.Activity => TreeModelUtility.CreateActivityTreeStructure(metricCollection),
                _ => throw new ArgumentOutOfRangeException(nameof(displayType), displayType, null)
            };
        }

        public override void Show()
        {
            CleanupExistingUI();
            SetupUIElements();
            ShowStandardTreeView();
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public override void Hide()
        {
            CleanupExistingUI();
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public override void CustomizeToolbar(VisualElement container)
        {
            m_SearchBar = new SearchBar();
            m_SearchBar.OnSearchStringCleared += HandleOnSearchStringCleared;
            m_SearchBar.OnSearchResultsChanged += HandleOnSearchResultsChanged;

            // Call set entries after adding the callback, so that we can
            // immediately get the callback with the filtered results
            m_SearchBar.SetEntries(m_TreeModel);
            container.Add(m_SearchBar);
        }

        void CleanupExistingUI()
        {
            CleanupSearchBar();
            CleanupColumnBar();
        }

        void CleanupColumnBar()
        {
            if (m_ColumnBarNetwork == null) return;
            m_ColumnBarNetwork.NameClickEvent -= HandleOnNameClickedEvent;
            m_ColumnBarNetwork.TypeClickEvent -= HandleOnTypeClickedEvent;
            m_ColumnBarNetwork.BytesSentClickEvent -= HandleOnBytesSentClickEvent;
            m_ColumnBarNetwork.BytesReceivedClickEvent -= HandleOnBytesReceivedClickEvent;
            m_ColumnBarNetwork.RemoveFromHierarchy();
        }

        void CleanupSearchBar()
        {
            if (m_SearchBar == null) return;
            m_SearchBar.OnSearchStringCleared -= HandleOnSearchStringCleared;
            m_SearchBar.OnSearchResultsChanged -= HandleOnSearchResultsChanged;
            m_SearchBar.RemoveFromHierarchy();
        }

        void SetupUIElements()
        {
            m_ColumnBarNetwork = new ColumnBarNetwork();
            m_ColumnBarNetwork.NameClickEvent += HandleOnNameClickedEvent;
            m_ColumnBarNetwork.TypeClickEvent += HandleOnTypeClickedEvent;
            m_ColumnBarNetwork.BytesReceivedClickEvent += HandleOnBytesReceivedClickEvent;
            m_ColumnBarNetwork.BytesSentClickEvent += HandleOnBytesSentClickEvent;
            m_TreeView = new TreeViewNetwork(m_TreeModel);
            m_TreeView.Show(m_DisplayType);
            m_TreeViewArea = new VisualElement {name = "TreeView Area"};
            m_TreeViewArea.style.flexGrow = 1;
            m_TreeViewArea.Add(m_TreeView);
            m_FilteredResultsArea = new ListViewContainer();
        }

        void HandleOnNameClickedEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.NameSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.NameSort(isAscending);
            }
        }

        void HandleOnTypeClickedEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.TypeSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.TypeSort(isAscending);
            }
        }

        void HandleOnBytesReceivedClickEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.BytesReceivedSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.BytesReceivedSort(isAscending);
            }
        }

        void HandleOnBytesSentClickEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.BytesSentSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.BytesSentSort(isAscending);
            }
        }

        void HandleOnSearchStringCleared()
        {
            ShowStandardTreeView();
        }

        void HandleOnSearchResultsChanged(IReadOnlyCollection<IRowData> results)
        {
            ShowFilteredResults(results);
        }

        void ShowStandardTreeView()
        {
            Clear();

            m_ShowStandard = true;
            m_ShowFiltered = false;
            Add(m_ColumnBarNetwork);
            Add(m_TreeViewArea);
            
            m_TreeView.RefreshSelected();
        }

        void ShowFilteredResults(IEnumerable<IRowData> results)
        {
            Clear();

            m_ShowFiltered = true;
            m_ShowStandard = false;
            Add(m_ColumnBarNetwork);
            m_FilteredResultsArea.CacheResults(results);
            m_FilteredResultsArea.ShowResults();
            Add(m_FilteredResultsArea);
            
            m_TreeView.RefreshSelected();
        }
    }
}
