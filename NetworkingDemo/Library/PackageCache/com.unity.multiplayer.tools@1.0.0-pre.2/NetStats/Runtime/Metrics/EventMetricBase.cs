using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    abstract class EventMetricBase<TValue> : IEventMetric<TValue>, IResettable
    {
        readonly List<TValue> m_Values = new List<TValue>();

        protected EventMetricBase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; }

        public IReadOnlyList<TValue> Values => m_Values;

        public bool ShouldResetOnDispatch { get; set; } = true;

        public void Mark(TValue value)
        {
            m_Values.Add(value);
        }

        public void Reset()
        {
            m_Values.Clear();
        }
    }
}
