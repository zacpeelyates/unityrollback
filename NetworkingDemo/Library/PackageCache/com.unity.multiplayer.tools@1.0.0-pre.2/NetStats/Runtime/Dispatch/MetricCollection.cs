using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    sealed class MetricCollection
    {
        IReadOnlyDictionary<string, IMetric<long>> m_Counters;
        IReadOnlyDictionary<string, IMetric<double>> m_Gauges;
        IReadOnlyDictionary<string, IMetric<TimeSpan>> m_Timers;
        IReadOnlyDictionary<string, IEventMetric<string>> m_Events;
        IReadOnlyDictionary<string, IEventMetric> m_PayloadEvents;

        internal MetricCollection(
            IReadOnlyDictionary<string, IMetric<long>> counters,
            IReadOnlyDictionary<string, IMetric<double>> gauges,
            IReadOnlyDictionary<string, IMetric<TimeSpan>> timers,
            IReadOnlyDictionary<string, IEventMetric<string>> events,
            IReadOnlyDictionary<string, IEventMetric> payloadEvents)
        {
            m_Counters = counters;
            m_Gauges = gauges;
            m_Timers = timers;
            m_Events = events;
            m_PayloadEvents = payloadEvents;

            Metrics = counters.Values
                .Concat<IMetric>(gauges.Values)
                .Concat(timers.Values)
                .Concat(m_Events.Values)
                .Concat(m_PayloadEvents.Values)
                .ToList();
        }

        public IReadOnlyCollection<IMetric> Metrics { get; }

        public ulong ConnectionId { get; set; } = ulong.MaxValue;

        public bool TryGetCounter(string name, out IMetric<long> counter)
        {
            return m_Counters.TryGetValue(name, out counter);
        }

        public bool TryGetGauge(string name, out IMetric<double> gauge)
        {
            return m_Gauges.TryGetValue(name, out gauge);
        }

        public bool TryGetTimer(string name, out IMetric<TimeSpan> timer)
        {
            return m_Timers.TryGetValue(name, out timer);
        }

        public bool TryGetEvent(string name, out IEventMetric<string> metricEvent)
        {
            return m_Events.TryGetValue(name, out metricEvent);
        }

        public bool TryGetEvent<TEvent>(string name, out IEventMetric<TEvent> metricEvent)
        {
            var found = m_PayloadEvents.TryGetValue(name, out var value);
            if (found && value is IEventMetric<TEvent> typedMetric)
            {
                metricEvent = typedMetric;
                return true;
            }

            metricEvent = null;
            return false;
        }
    }
}
