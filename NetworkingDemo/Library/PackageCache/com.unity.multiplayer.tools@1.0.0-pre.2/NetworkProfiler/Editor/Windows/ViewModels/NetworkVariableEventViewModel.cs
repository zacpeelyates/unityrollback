using System;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class NetworkVariableEventViewModel : ViewModelBase
    {
        public NetworkVariableEventViewModel(string componentName, string variableName, IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                $"{componentName}.{variableName}",
                MetricType.NetworkVariableDelta,
                onSelectedCallback)
        {
        }
    }
}