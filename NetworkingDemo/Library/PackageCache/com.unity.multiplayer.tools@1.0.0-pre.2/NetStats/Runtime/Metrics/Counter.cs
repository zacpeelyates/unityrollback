using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    class Counter : Metric<long>
    {
        public Counter(string name, long defaultValue = default)
            : base(name, defaultValue)
        {
        }

        public void Increment(long increment = 1)
        {
            Value += increment;
        }
    }
}
