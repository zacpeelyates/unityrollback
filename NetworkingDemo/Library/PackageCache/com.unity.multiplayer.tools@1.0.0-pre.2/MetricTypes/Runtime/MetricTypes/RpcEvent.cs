using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct RpcEvent : INetworkMetricEvent, INetworkObjectEvent
    {
        public RpcEvent(ConnectionInfo connection, NetworkObjectIdentifier networkId, string name, string networkBehaviourName, long bytesCount)
        {
            Connection = connection;
            NetworkId = networkId;
            Name = name;
            NetworkBehaviourName = networkBehaviourName;
            BytesCount = bytesCount;
        }

        public ConnectionInfo Connection { get; }

        public NetworkObjectIdentifier NetworkId { get; }

        public string Name { get; }
        
        public string NetworkBehaviourName { get; }

        public long BytesCount { get; }
    }
}