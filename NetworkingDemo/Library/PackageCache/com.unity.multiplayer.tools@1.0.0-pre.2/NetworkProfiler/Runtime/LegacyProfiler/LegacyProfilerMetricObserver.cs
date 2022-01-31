#if !UNITY_2021_2_OR_NEWER
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetworkProfiler;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class LegacyProfilerMetricObserver : IMetricObserver
    {
        public void Observe(MetricCollection collection)
        {
            ProfilerCounters.Instance.UpdateFromMetrics(collection);
        }
    }
}
#endif