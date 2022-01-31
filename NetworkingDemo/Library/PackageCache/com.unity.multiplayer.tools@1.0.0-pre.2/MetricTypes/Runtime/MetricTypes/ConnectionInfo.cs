using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct ConnectionInfo
    {
        public ConnectionInfo(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }

        public static bool operator==(ConnectionInfo a, ConnectionInfo b)
        {
            return a.Equals(b);
        }
        public static bool operator!=(ConnectionInfo a, ConnectionInfo b)
        {
            return !(a == b);
        }
        public bool Equals(ConnectionInfo other)
        {
            return Id == other.Id;
        }
        public override bool Equals(object obj)
        {
            return obj is ConnectionInfo other && Equals(other);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}