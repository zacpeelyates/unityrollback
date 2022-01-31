using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct NetworkObjectIdentifier
    {
        public NetworkObjectIdentifier(string name, ulong networkId)
        {
            Name = name;
            NetworkId = networkId;
        }

        public string Name { get; }

        public ulong NetworkId { get; }
    }
}