using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unity.Multiplayer.Tools.NetStats
{
    sealed class MetricDispatcherBuilder
    {
        readonly IDictionary<string, IMetric<long>> m_Counters = new Dictionary<string, IMetric<long>>();
        readonly IDictionary<string, IMetric<double>> m_Gauges = new Dictionary<string, IMetric<double>>();
        readonly IDictionary<string, IMetric<TimeSpan>> m_Timers = new Dictionary<string, IMetric<TimeSpan>>();
        readonly IDictionary<string, IEventMetric<string>> m_Events = new Dictionary<string, IEventMetric<string>>();
        readonly IDictionary<string, IEventMetric> m_PayloadEvents = new Dictionary<string, IEventMetric>();

        readonly List<IResettable> m_Resettables = new List<IResettable>();

        public MetricDispatcherBuilder WithCounters(params Counter[] counters)
        {
            foreach (var counter in counters)
            {
                m_Counters[counter.Name] = counter;
                m_Resettables.Add(counter);
            }

            return this;
        }

        public MetricDispatcherBuilder WithGauges(params Gauge[] gauges)
        {
            foreach (var gauge in gauges)
            {
                m_Gauges[gauge.Name] = gauge;
                m_Resettables.Add(gauge);
            }

            return this;
        }

        public MetricDispatcherBuilder WithTimers(params Timer[] timers)
        {
            foreach (var timer in timers)
            {
                m_Timers[timer.Name] = timer;
                m_Resettables.Add(timer);
            }

            return this;
        }

        public MetricDispatcherBuilder WithMetricEvents(params EventMetric[] metricEvents)
        {
            foreach (var metricEvent in metricEvents)
            {
                m_Events[metricEvent.Name] = metricEvent;
                m_Resettables.Add(metricEvent);
            }

            return this;
        }

        public MetricDispatcherBuilder WithMetricEvents<TEvent>(params EventMetric<TEvent>[] metricEvents)
            where TEvent : struct
        {
            foreach (var metricEvent in metricEvents)
            {
                m_PayloadEvents[metricEvent.Name] = metricEvent;
                m_Resettables.Add(metricEvent);
            }

            return this;
        }

        public IMetricDispatcher Build()
        {
            return new MetricDispatcher(
                new MetricCollection(
                    new ReadOnlyDictionary<string, IMetric<long>>(m_Counters),
                    new ReadOnlyDictionary<string, IMetric<double>>(m_Gauges),
                    new ReadOnlyDictionary<string, IMetric<TimeSpan>>(m_Timers),
                    new ReadOnlyDictionary<string, IEventMetric<string>>(m_Events),
                    new ReadOnlyDictionary<string, IEventMetric>(m_PayloadEvents)),
                m_Resettables);
        }
    }
}