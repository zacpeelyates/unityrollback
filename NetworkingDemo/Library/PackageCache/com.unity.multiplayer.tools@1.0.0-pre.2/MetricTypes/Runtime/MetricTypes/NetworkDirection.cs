using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Flags]
    enum NetworkDirection
    {
        None = 0,
        Received = 1,
        Sent = 2,
        Both = Received | Sent,
    }

    static class NetworkDirectionExtensions
    {
        public static string DisplayString(this NetworkDirection direction)
        {
            return direction == NetworkDirection.None
                ? ""
                : direction.ToString();
        }
    }
}