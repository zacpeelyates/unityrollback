using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct NamedMessageEvent : INetworkMetricEvent
    {
        public NamedMessageEvent(ConnectionInfo connection, string name, long bytesCount)
        {
            Connection = connection;
            Name = name;
            BytesCount = bytesCount;
        }

        public ConnectionInfo Connection { get; }

        public string Name { get; }

        public long BytesCount { get; }
    }
}