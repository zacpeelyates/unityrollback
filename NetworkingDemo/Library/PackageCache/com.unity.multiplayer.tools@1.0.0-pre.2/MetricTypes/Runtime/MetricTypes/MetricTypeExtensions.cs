using System.Linq;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    static class MetricTypeExtensions
    {
        internal static string GetDisplayNameString(this MetricType metricType)
        {
            return string.Concat(metricType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }

        internal static string GetTypeNameString(this MetricType metricType)
        {
            return metricType.ToString().ToLowerInvariant();
        }
    }
}