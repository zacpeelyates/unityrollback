#if UNITY_2021_2_OR_NEWER
using System.Diagnostics;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine.Profiling;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class ExtensibilityProfilerMetricObserver : IMetricObserver
    {
        readonly INetStatSerializer m_NetStatSerializer;

        public ExtensibilityProfilerMetricObserver()
        {
            m_NetStatSerializer = new NetStatSerializer();
        }
        public void Observe(MetricCollection collection)
        {
            PopulateProfilerIfEnabled(collection);
        }

        [Conditional("ENABLE_PROFILER")]
        void PopulateProfilerIfEnabled(MetricCollection collection)
        {
            ProfilerCounters.Instance.UpdateFromMetrics(collection);

            using var result = m_NetStatSerializer.Serialize(collection);
            Profiler.EmitFrameMetaData(
                FrameInfo.NetworkProfilerGuid, 
                FrameInfo.NetworkProfilerDataTag, 
                result);
        }
    }
}
#endif