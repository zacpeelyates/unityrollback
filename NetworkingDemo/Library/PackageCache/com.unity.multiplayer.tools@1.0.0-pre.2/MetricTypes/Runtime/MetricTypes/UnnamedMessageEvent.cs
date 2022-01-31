using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct UnnamedMessageEvent : INetworkMetricEvent
    {
        public UnnamedMessageEvent(ConnectionInfo connection, long bytesCount)
        {
            Connection = connection;
            BytesCount = bytesCount;
        }

        public ConnectionInfo Connection { get; }

        public long BytesCount { get; }
    }
}