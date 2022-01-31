using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class MetricCounters
    {
        public readonly MetricByteCounters Bytes;
        public readonly MetricEventCounters Events;

        public MetricCounters(
            string displayName, 
            ICounterFactory byteCounterFactory, 
            ICounterFactory eventCounterFactory)
        {
            Bytes = new MetricByteCounters(displayName, byteCounterFactory);
            Events = new MetricEventCounters(displayName, eventCounterFactory);
        }

        public void Sample<TEventData>(
            IReadOnlyList<TEventData> sent, 
            IReadOnlyList<TEventData> received) 
            where TEventData : struct, INetworkMetricEvent
        {
            Bytes.Sample(sent, received);
            Events.Sample(sent, received);
        }
    }
}