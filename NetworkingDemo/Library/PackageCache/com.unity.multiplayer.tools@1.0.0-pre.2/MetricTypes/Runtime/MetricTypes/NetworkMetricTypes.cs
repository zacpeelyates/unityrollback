namespace Unity.Multiplayer.Tools.MetricTypes
{
    static class NetworkMetricTypes
    {
        public static readonly DirectionalMetricInfo NetworkMessageSent = new DirectionalMetricInfo(MetricType.NetworkMessage, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo NetworkMessageReceived = new DirectionalMetricInfo(MetricType.NetworkMessage, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo TotalBytesSent = new DirectionalMetricInfo(MetricType.TotalBytes, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo TotalBytesReceived = new DirectionalMetricInfo(MetricType.TotalBytes, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo RpcSent = new DirectionalMetricInfo(MetricType.Rpc, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo RpcReceived = new DirectionalMetricInfo(MetricType.Rpc, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo NamedMessageSent = new DirectionalMetricInfo(MetricType.NamedMessage, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo NamedMessageReceived = new DirectionalMetricInfo(MetricType.NamedMessage, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo UnnamedMessageSent = new DirectionalMetricInfo(MetricType.UnnamedMessage, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo UnnamedMessageReceived = new DirectionalMetricInfo(MetricType.UnnamedMessage, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo NetworkVariableDeltaSent = new DirectionalMetricInfo(MetricType.NetworkVariableDelta, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo NetworkVariableDeltaReceived = new DirectionalMetricInfo(MetricType.NetworkVariableDelta, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo ObjectSpawnedSent = new DirectionalMetricInfo(MetricType.ObjectSpawned, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo ObjectSpawnedReceived = new DirectionalMetricInfo(MetricType.ObjectSpawned, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo ObjectDestroyedSent = new DirectionalMetricInfo(MetricType.ObjectDestroyed, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo ObjectDestroyedReceived = new DirectionalMetricInfo(MetricType.ObjectDestroyed, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo OwnershipChangeSent = new DirectionalMetricInfo(MetricType.OwnershipChange, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo OwnershipChangeReceived = new DirectionalMetricInfo(MetricType.OwnershipChange, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo ServerLogSent = new DirectionalMetricInfo(MetricType.ServerLog, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo ServerLogReceived = new DirectionalMetricInfo(MetricType.ServerLog, NetworkDirection.Received);
        public static readonly DirectionalMetricInfo SceneEventSent = new DirectionalMetricInfo(MetricType.SceneEvent, NetworkDirection.Sent);
        public static readonly DirectionalMetricInfo SceneEventReceived = new DirectionalMetricInfo(MetricType.SceneEvent, NetworkDirection.Received);
    }
}