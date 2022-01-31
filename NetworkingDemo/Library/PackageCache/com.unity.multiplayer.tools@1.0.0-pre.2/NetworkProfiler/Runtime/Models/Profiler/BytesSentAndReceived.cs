using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    [Serializable]
    internal struct BytesSentAndReceived : IEquatable<BytesSentAndReceived>
    {
        public BytesSentAndReceived(long sent = 0, long received = 0)
        {
            Sent = sent;
            Received = received;
        }

        public long Sent { get; }
        public long Received { get; }

        public NetworkDirection Direction =>
            (Sent     > 0f ? NetworkDirection.Sent : NetworkDirection.None) |
            (Received > 0f ? NetworkDirection.Received : NetworkDirection.None);

        public long Total => Sent + Received;

        public bool Equals(BytesSentAndReceived other)
        {
            return Sent == other.Sent &&
                   Received == other.Received;
        }

        public override bool Equals(object obj)
        {
            return obj is BytesSentAndReceived other && Equals(other);
        }

        public static BytesSentAndReceived operator +(
            BytesSentAndReceived a,
            BytesSentAndReceived b) => new BytesSentAndReceived(
                a.Sent + b.Sent,
                a.Received + b.Received);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Sent.GetHashCode() * 397) ^ Received.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{nameof(BytesSentAndReceived)}: {nameof(Sent)}={Sent} {nameof(Received)}={Received}";
        }
    }

    internal static class BytesSentAndReceivedExtensions
    {
        public static BytesSentAndReceived Sum<T>(this IEnumerable<T> ts, Func<T, BytesSentAndReceived> f)
        {
            BytesSentAndReceived result = new BytesSentAndReceived();
            foreach (var t in ts)
            {
                result += f(t);
            }
            return result;
        }
    }
}
