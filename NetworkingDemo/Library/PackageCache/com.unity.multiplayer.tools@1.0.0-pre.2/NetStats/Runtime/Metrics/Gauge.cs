using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    class Gauge : Metric<double>
    {
        public Gauge(string name, double defaultValue = default)
            : base(name, defaultValue)
        {
        }

        public void Set(double value)
        {
            Value = value;
        }
    }
}
