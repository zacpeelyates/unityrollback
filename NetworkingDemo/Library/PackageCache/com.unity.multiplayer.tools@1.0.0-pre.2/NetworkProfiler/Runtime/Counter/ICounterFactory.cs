namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    interface ICounterFactory
    {
        ICounter Construct(string name);
    }
}
