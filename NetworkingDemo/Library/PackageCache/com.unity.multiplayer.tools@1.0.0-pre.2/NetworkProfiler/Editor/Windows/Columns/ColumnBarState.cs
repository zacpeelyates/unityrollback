namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class ColumnBarState
    {
        public bool IsNameAscending { get; private set; }
        public bool IsTypeAscending { get; private set; }
        public bool IsBytesReceivedAscending { get; private set; }
        public bool IsBytesSentAscending { get; private set; }

        void Reset()
        {
            IsNameAscending = false;
            IsTypeAscending = false;
            IsBytesSentAscending = false;
            IsBytesReceivedAscending = false;
        }

        internal bool ToggleNameSortDirection()
        {
            var ascending = IsNameAscending;
            Reset();
            IsNameAscending = !ascending;
            return ascending;
        }

        internal bool ToggleTypeSortDirection()
        {
            var ascending = IsTypeAscending;
            Reset();
            IsTypeAscending = !ascending;
            return ascending;
        }

        internal bool ToggleBytesSentSortDirection()
        {
            var ascending = IsBytesSentAscending;
            Reset();
            IsBytesSentAscending = !ascending;
            return ascending;
        }

        internal bool ToggleBytesReceivedSortDirection()
        {
            var ascending = IsBytesReceivedAscending;
            Reset();
            IsBytesReceivedAscending = !ascending;
            return ascending;
        }
    }
}