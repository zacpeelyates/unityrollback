using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats
{
    class MetricCollectionBuilder
    {
        readonly List<IMetric<long>> m_Counters = new List<IMetric<long>>();
        readonly List<IMetric<double>> m_Gauges = new List<IMetric<double>>();
        readonly List<IMetric<TimeSpan>> m_Timers = new List<IMetric<TimeSpan>>();
        readonly List<IEventMetric<string>> m_Events = new List<IEventMetric<string>>();
        readonly List<IEventMetric> m_PayloadEvents = new List<IEventMetric>();

        public MetricCollectionBuilder WithCounters(params Counter[] counters)
        {
            m_Counters.AddRange(counters);

            return this;
        }

        public MetricCollectionBuilder WithGauges(params Gauge[] gauges)
        {
            m_Gauges.AddRange(gauges);

            return this;
        }

        public MetricCollectionBuilder WithTimers(params Timer[] timers)
        {
            m_Timers.AddRange(timers);

            return this;
        }

        public MetricCollectionBuilder WithMetricEvents(params IEventMetric<string>[] metricEvents)
        {
            m_Events.AddRange(metricEvents);

            return this;
        }

        public MetricCollectionBuilder WithMetricEvents<TEvent>(params IEventMetric<TEvent>[] metricEvents)
            where TEvent : struct
        {
            m_PayloadEvents.AddRange(metricEvents);

            return this;
        }

        public MetricCollection Build()
        {
            return new MetricCollection(
                new ReadOnlyDictionary<string, IMetric<long>>(m_Counters.ToDictionary(x => x.Name, x => x)),
                new ReadOnlyDictionary<string, IMetric<double>>(m_Gauges.ToDictionary(x => x.Name, x => x)),
                new ReadOnlyDictionary<string, IMetric<TimeSpan>>(m_Timers.ToDictionary(x => x.Name, x => x)),
                new ReadOnlyDictionary<string, IEventMetric<string>>(m_Events.ToDictionary(x => x.Name, x => x)),
                new ReadOnlyDictionary<string, IEventMetric>(m_PayloadEvents.ToDictionary(x => x.Name, x => x)));
        }
    }
}