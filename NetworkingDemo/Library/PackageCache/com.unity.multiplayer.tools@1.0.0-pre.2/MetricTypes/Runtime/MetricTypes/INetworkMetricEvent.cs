namespace Unity.Multiplayer.Tools.MetricTypes
{
    interface INetworkMetricEvent
    {
        ConnectionInfo Connection { get; }

        long BytesCount { get; }
    }
}