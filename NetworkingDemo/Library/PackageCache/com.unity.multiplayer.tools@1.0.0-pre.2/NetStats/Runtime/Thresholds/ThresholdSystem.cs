using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
    class ThresholdSystem : IMetricDispatcher, IMetricObserver
    {
        const string k_ThresholdEventMetricName = "Threshold Events";

        readonly IDictionary<string, IList<ThresholdHandler>> m_Handlers = new Dictionary<string, IList<ThresholdHandler>>();

        readonly IList<IMetricObserver> m_Observers = new List<IMetricObserver>();

        readonly EventMetric<ThresholdAlert> m_ThresholdAlertMetric;
        readonly MetricCollection m_Collection;

        public ThresholdSystem()
        {
            m_ThresholdAlertMetric = new EventMetric<ThresholdAlert>(k_ThresholdEventMetricName)
            {
                ShouldResetOnDispatch = true,
            };

            m_Collection = new MetricCollectionBuilder()
                .WithMetricEvents(m_ThresholdAlertMetric)
                .Build();
        }

        public void Observe(MetricCollection collection)
        {
            foreach (var metric in collection.Metrics)
            {
                if (!m_Handlers.TryGetValue(metric.Name, out var handlers))
                {
                    continue;
                }

                foreach (var handler in handlers)
                {
                    if (handler.Configuration.IsConditionMet(metric))
                    {
                        handler.OnConditionTriggered.Invoke(metric);
                        m_ThresholdAlertMetric.Mark(new ThresholdAlert(metric, handler.Configuration));
                    }
                }
            }

            Dispatch();
        }

        public void RegisterCondition<TValue>(string statName, Func<TValue, bool> threshold, Action<IMetric> onConditionTriggered)
        {
            RegisterCondition<IMetric<TValue>, TValue>(statName, (stat) => stat.Value, threshold, onConditionTriggered);
        }

        public void RegisterCondition<TStat, TValue>(string statName, Func<TStat, TValue> valueProvider, Func<TValue, bool> threshold, Action<IMetric> onConditionTriggered)
        {
            if (!m_Handlers.TryGetValue(statName, out var values))
            {
                values = new List<ThresholdHandler>();
                m_Handlers[statName] = values;
            }

            values.Add(new ThresholdHandler(new ThresholdConfiguration<TStat, TValue>(valueProvider, threshold), onConditionTriggered));
        }

        public void RegisterObserver(IMetricObserver observer)
        {
            m_Observers.Add(observer);
        }

        public void SetConnectionId(ulong connectionId)
        {
        }

        public void Dispatch()
        {
            foreach (var observer in m_Observers)
            {
                observer.Observe(m_Collection);
            }

            m_ThresholdAlertMetric.Reset();
        }

        class ThresholdHandler
        {
            public ThresholdHandler(IThresholdConfiguration configuration, Action<IMetric> onConditionTriggered)
            {
                Configuration = configuration;
                OnConditionTriggered = onConditionTriggered;
            }
            
            public IThresholdConfiguration Configuration { get; }

            public Action<IMetric> OnConditionTriggered { get; }
        }
    }
}