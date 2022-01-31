using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools
{
    static class MetricObserverFactory
    {
        internal static IMetricObserver Construct() => ProfilerMetricObserverFactory.Construct();
    }
}
