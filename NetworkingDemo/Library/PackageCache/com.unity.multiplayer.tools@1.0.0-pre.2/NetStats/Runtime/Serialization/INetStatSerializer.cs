using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
    internal interface INetStatSerializer
    {
        NativeArray<byte> Serialize(MetricCollection metricCollection);

        MetricCollection Deserialize(NativeArray<byte> bytes);
    }
}
