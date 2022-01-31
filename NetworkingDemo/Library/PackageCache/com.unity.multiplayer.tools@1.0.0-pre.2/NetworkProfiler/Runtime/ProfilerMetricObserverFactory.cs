using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    static class ProfilerMetricObserverFactory
    {
        public static IMetricObserver Construct()
        {
#if UNITY_2021_2_OR_NEWER
            return new ExtensibilityProfilerMetricObserver();
#else
            return new LegacyProfilerMetricObserver();
#endif
        }
    }
}