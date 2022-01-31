using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    class EventMetric : EventMetricBase<string>
    {
        public EventMetric(string name)
            : base(name)
        {
        }
    }

    [Serializable]
    class EventMetric<TValue> : EventMetricBase<TValue>
        where TValue : struct
    {
        public EventMetric(string name)
            : base(name)
        {
        }
    }
}
