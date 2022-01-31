using System;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class OwnershipChangeEventViewModel : ViewModelBase
    {
        public OwnershipChangeEventViewModel(IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                $"{MetricType.OwnershipChange.GetDisplayNameString()}",
                MetricType.OwnershipChange,
                onSelectedCallback)
        {
        }
    }
}