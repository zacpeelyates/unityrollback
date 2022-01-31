using System;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class SpawnEventViewModel : ViewModelBase
    {
        public SpawnEventViewModel(NetworkObjectIdentifier objectId, IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                $"{MetricType.ObjectSpawned.GetDisplayNameString()}",
                MetricType.ObjectSpawned,
                onSelectedCallback)
        {
        }
    }
}