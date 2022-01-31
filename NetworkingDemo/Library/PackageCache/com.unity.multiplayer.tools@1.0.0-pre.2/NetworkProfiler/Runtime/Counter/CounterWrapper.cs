using Unity.Profiling;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class CounterWrapper : ICounter
    {
        ProfilerCounter<long> m_Counter;
        
        public CounterWrapper(ProfilerCounter<long> counter)
        {
            m_Counter = counter;
        }
        
        public void Sample(long inValue)
        {
            m_Counter.Sample(inValue);
        }
    }
}
