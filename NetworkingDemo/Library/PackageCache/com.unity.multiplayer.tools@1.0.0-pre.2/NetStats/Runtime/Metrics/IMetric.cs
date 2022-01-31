namespace Unity.Multiplayer.Tools.NetStats
{
    interface IMetric
    {
        string Name { get; }
    }

    interface IMetric<TValue> : IMetric
    {
        TValue Value { get; }
    }
}
