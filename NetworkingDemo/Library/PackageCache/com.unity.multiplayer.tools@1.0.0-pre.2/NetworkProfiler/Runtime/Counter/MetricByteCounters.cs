using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class MetricByteCounters
    {
        readonly ICounter m_SentCounter;
        readonly ICounter m_ReceivedCounter;

        public MetricByteCounters(string displayName, ICounterFactory counterFactory)
        {
            Sent = $"{displayName} Bytes Sent";
            Received = $"{displayName} Bytes Received";

            m_SentCounter = counterFactory.Construct(Sent);
            m_ReceivedCounter = counterFactory.Construct(Received);
        }
        
        public string Sent { get; }

        public string Received { get; }

        public void Sample<TEventData>(
            IReadOnlyList<TEventData> sentMetrics, 
            IReadOnlyList<TEventData> receivedMetrics) 
            where TEventData : struct, INetworkMetricEvent
        {
            var sentValue  = 0L;
            for (var i = 0; i < sentMetrics.Count; i++)
            {
                var metric = sentMetrics[i];
                sentValue += metric.BytesCount;
            }

            var receivedValue = 0L;
            for (var i = 0; i < receivedMetrics.Count; i++)
            {
                var metric = receivedMetrics[i];
                receivedValue += metric.BytesCount;
            }

            Sample(sentValue, receivedValue);
        }

        public void Sample(long sent, long received)
        {
            m_SentCounter.Sample(sent);
            m_ReceivedCounter.Sample(received);
        }
    }
}