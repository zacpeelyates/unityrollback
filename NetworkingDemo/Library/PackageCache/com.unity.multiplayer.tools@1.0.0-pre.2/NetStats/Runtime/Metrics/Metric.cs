using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    abstract class Metric<TValue> : IMetric<TValue>, IResettable
    {
        protected Metric(string name, TValue defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public string Name { get; }

        public TValue Value { get; protected set; }

        protected TValue DefaultValue { get; }

        public bool ShouldResetOnDispatch { get; set; } = true;

        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
