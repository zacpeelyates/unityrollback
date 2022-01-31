using Unity.Profiling;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class EventCounterFactory : ICounterFactory
    {
        public ICounter Construct(string name)
            => new CounterWrapper(
                new ProfilerCounter<long>(
                    ProfilerCategory.Network, name, ProfilerMarkerDataUnit.Count));
    }
}
