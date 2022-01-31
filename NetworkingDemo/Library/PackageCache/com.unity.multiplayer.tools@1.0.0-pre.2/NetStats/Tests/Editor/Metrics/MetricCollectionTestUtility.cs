using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    static class MetricCollectionTestUtility
    {
        public static MetricCollection ConstructFromMetrics(
            IReadOnlyCollection<IMetric> metrics,
             ulong localConnectionId = ulong.MaxValue)
        {
            static string ByName(IMetric metric) => metric.Name;
            var metricCollection = new MetricCollection(
                metrics.OfType<IMetric<long>>().ToDictionary(ByName),
                metrics.OfType<IMetric<double>>().ToDictionary(ByName),
                metrics.OfType<IMetric<TimeSpan>>().ToDictionary(ByName),
                metrics.OfType<IEventMetric<string>>().ToDictionary(ByName),
                metrics.OfType<IEventMetric>().ToDictionary(ByName));
            metricCollection.ConnectionId = localConnectionId;

            return metricCollection;
        }
    }
}
