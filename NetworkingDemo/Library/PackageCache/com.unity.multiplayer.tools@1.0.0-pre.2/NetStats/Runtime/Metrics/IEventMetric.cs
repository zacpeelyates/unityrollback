using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
    interface IEventMetric : IMetric
    {
    }

    interface IEventMetric<TValue> : IEventMetric
    {
        IReadOnlyList<TValue> Values { get; }
    }
}