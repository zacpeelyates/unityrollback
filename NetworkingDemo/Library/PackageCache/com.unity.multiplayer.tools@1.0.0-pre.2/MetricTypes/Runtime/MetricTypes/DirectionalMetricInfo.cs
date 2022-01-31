using System.Linq;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    struct DirectionalMetricInfo
    {
        public DirectionalMetricInfo(MetricType metricType, NetworkDirection networkDirection)
        {
            Type = metricType;
            Direction = networkDirection;

            Id = $"{Type.ToString()}{Direction.ToString()}".ToLowerInvariant();
            DisplayName = $"{Type.GetDisplayNameString()} {Direction.ToString()}";
        }

        internal MetricType Type { get; }

        internal NetworkDirection Direction { get; }

        internal string Id { get; }

        internal string DisplayName { get; }
    }
}