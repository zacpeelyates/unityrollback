using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class ListViewContainer : VisualElement
    {
        List<IRowData> m_CachedEntries;

        public ListViewContainer()
        {
            style.flexGrow = 1;
        }

        internal void CacheResults(IEnumerable<IRowData> results)
        {
            m_CachedEntries = new List<IRowData>(results);
        }

        internal void ShowResults()
        {
            ClearResults();
            Add(new ListViewNetwork(m_CachedEntries));
        }

        internal void ClearResults()
        {
            Clear();
            RemoveFromHierarchy();
        }

        internal void NameSort(bool isAscending)
        {
            var sort = isAscending
                ? SortDirection.NameAscending
                : SortDirection.NameDescending;
            SortShowResults(sort);
        }

        internal void TypeSort(bool isAscending)
        {
            var sort = isAscending
                ? SortDirection.TypeAscending
                : SortDirection.TypeDescending;
            SortShowResults(sort);
        }

        internal void BytesSentSort(bool isAscending)
        {
            var sort = isAscending
                ? SortDirection.BytesSentAscending
                : SortDirection.BytesSentDescending;
            SortShowResults(sort);
        }

        internal void BytesReceivedSort(bool isAscending)
        {
            var sort = isAscending
                ? SortDirection.BytesReceivedAscending
                : SortDirection.BytesReceivedDescending;
            SortShowResults(sort);
        }

        void SortShowResults(SortDirection sortDirection)
        {
            RowDataSorting.Sort(m_CachedEntries, sortDirection);
            ShowResults();
        }
    }
}