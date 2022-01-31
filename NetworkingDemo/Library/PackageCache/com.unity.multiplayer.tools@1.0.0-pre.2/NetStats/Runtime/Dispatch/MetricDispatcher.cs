using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
    class MetricDispatcher : IMetricDispatcher
    {
        readonly MetricCollection m_Collection;
        readonly IReadOnlyList<IResettable> m_Resettables;

        readonly IList<IMetricObserver> m_Observers = new List<IMetricObserver>();

        internal MetricDispatcher(
            MetricCollection collection,
            IReadOnlyList<IResettable> resettables)
        {
            m_Collection = collection;
            m_Resettables = resettables;
        }

        public void RegisterObserver(IMetricObserver observer)
        {
            m_Observers.Add(observer);
        }

        public void SetConnectionId(ulong connectionId)
        {
            m_Collection.ConnectionId = connectionId;
        }

        public void Dispatch()
        {
            for (var i = 0; i < m_Observers.Count; i++)
            {
                var snapshotObserver = m_Observers[i];
                snapshotObserver.Observe(m_Collection);
            }

            for (var i = 0; i < m_Resettables.Count; i++)
            {
                var resettable = m_Resettables[i];
                if (resettable.ShouldResetOnDispatch)
                {
                    resettable.Reset();
                }
            }
        }
    }
}