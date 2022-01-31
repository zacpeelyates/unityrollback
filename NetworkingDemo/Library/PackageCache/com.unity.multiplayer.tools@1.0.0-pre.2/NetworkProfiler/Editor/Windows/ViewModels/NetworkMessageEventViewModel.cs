using System;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class NetworkMessageEventViewModel : ViewModelBase
    {
        public NetworkMessageEventViewModel(string messageName, IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                messageName,
                MetricType.NetworkMessage,
                onSelectedCallback)
        {
        }
    }
}