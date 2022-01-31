using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
    static class MetricsCollectionExtensions
    {
        public static IReadOnlyList<TMetric> GetEventValues<TMetric>(this MetricCollection collection, string metricName)
        {
            return collection.TryGetEvent<TMetric>(metricName, out var metric)
                ? metric.Values
                : Array.Empty<TMetric>();
        }
    }
}
