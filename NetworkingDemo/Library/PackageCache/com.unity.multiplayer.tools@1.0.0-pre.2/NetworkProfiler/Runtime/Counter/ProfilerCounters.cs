using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class ProfilerCounters
    {
        static ProfilerCounters s_Singleton;
        public static ProfilerCounters Instance => s_Singleton ??= new ProfilerCounters();
        
        public readonly MetricByteCounters totalBytes;
        public readonly MetricCounters rpc;
        public readonly MetricCounters namedMessage;
        public readonly MetricCounters unnamedMessage;
        public readonly MetricCounters networkVariableDelta;
        public readonly MetricCounters objectSpawned;
        public readonly MetricCounters objectDestroyed;
        public readonly MetricCounters serverLog;
        public readonly MetricCounters sceneEvent;
        public readonly MetricCounters ownershipChange;
        public readonly MetricCounters customMessage;
        public readonly MetricCounters networkMessage;

        ICounterFactory m_ByteCounterFactory;
        ICounterFactory m_EventCounterFactory;
        
        public ProfilerCounters(
            ICounterFactory byteCounterFactory = null, 
            ICounterFactory eventCounterFactory = null)
        {
            m_ByteCounterFactory = byteCounterFactory ?? new ByteCounterFactory();
            m_EventCounterFactory = eventCounterFactory ?? new EventCounterFactory();

            totalBytes = ConstructMetricByteCounters("Total");
            rpc = ConstructMetricCounters(MetricType.Rpc);
            namedMessage = ConstructMetricCounters(MetricType.NamedMessage);
            unnamedMessage = ConstructMetricCounters(MetricType.UnnamedMessage);
            networkVariableDelta = ConstructMetricCounters("Network Variable");
            objectSpawned = ConstructMetricCounters(MetricType.ObjectSpawned);
            objectDestroyed = ConstructMetricCounters(MetricType.ObjectDestroyed);
            serverLog = ConstructMetricCounters(MetricType.ServerLog);
            sceneEvent = ConstructMetricCounters(MetricType.SceneEvent);
            ownershipChange = ConstructMetricCounters(MetricType.OwnershipChange);
            customMessage = ConstructMetricCounters("Custom");
            networkMessage = ConstructMetricCounters("Network Messages");
        }

        MetricByteCounters ConstructMetricByteCounters(string name)
            => new MetricByteCounters(
                name,
                m_ByteCounterFactory);

        MetricCounters ConstructMetricCounters(MetricType metricType)
            => ConstructMetricCounters(metricType.GetDisplayNameString());
        MetricCounters ConstructMetricCounters(string name)
            => new MetricCounters(name, m_ByteCounterFactory, m_EventCounterFactory);

        public void UpdateFromMetrics(MetricCollection collection)
        {
            totalBytes.Sample(
                collection.TryGetCounter(NetworkMetricTypes.TotalBytesSent.Id, out var bytesSent)
                    ? bytesSent.Value
                    : 0L,
                collection.TryGetCounter(NetworkMetricTypes.TotalBytesReceived.Id, out var bytesReceived)
                    ? bytesReceived.Value 
                    : 0L);
            
            rpc.Sample(
                collection.GetEventValues<RpcEvent>(NetworkMetricTypes.RpcSent.Id),
                collection.GetEventValues<RpcEvent>(NetworkMetricTypes.RpcReceived.Id));
            
            namedMessage.Sample(
                collection.GetEventValues<NamedMessageEvent>(NetworkMetricTypes.NamedMessageSent.Id),
                collection.GetEventValues<NamedMessageEvent>(NetworkMetricTypes.NamedMessageReceived.Id));
            
            unnamedMessage.Sample(
                collection.GetEventValues<UnnamedMessageEvent>(NetworkMetricTypes.UnnamedMessageSent.Id),
                collection.GetEventValues<UnnamedMessageEvent>(NetworkMetricTypes.UnnamedMessageReceived.Id));
            
            // Custom messages is a combination of named and unnamed messages
            customMessage.Sample(
                collection.GetEventValues<NamedMessageEvent>(NetworkMetricTypes.NamedMessageSent.Id),
                collection.GetEventValues<NamedMessageEvent>(NetworkMetricTypes.NamedMessageReceived.Id));
            customMessage.Sample(
                collection.GetEventValues<UnnamedMessageEvent>(NetworkMetricTypes.UnnamedMessageSent.Id),
                collection.GetEventValues<UnnamedMessageEvent>(NetworkMetricTypes.UnnamedMessageReceived.Id));

            networkVariableDelta.Sample(
                collection.GetEventValues<NetworkVariableEvent>(NetworkMetricTypes.NetworkVariableDeltaSent.Id),
                collection.GetEventValues<NetworkVariableEvent>(NetworkMetricTypes.NetworkVariableDeltaReceived.Id));
            
            objectSpawned.Sample(
                collection.GetEventValues<ObjectSpawnedEvent>(NetworkMetricTypes.ObjectSpawnedSent.Id),
                collection.GetEventValues<ObjectSpawnedEvent>(NetworkMetricTypes.ObjectSpawnedReceived.Id));

            objectDestroyed.Sample(
                collection.GetEventValues<ObjectDestroyedEvent>(NetworkMetricTypes.ObjectDestroyedSent.Id),
                collection.GetEventValues<ObjectDestroyedEvent>(NetworkMetricTypes.ObjectDestroyedReceived.Id));

            serverLog.Sample(
                collection.GetEventValues<ServerLogEvent>(NetworkMetricTypes.ServerLogSent.Id),
                collection.GetEventValues<ServerLogEvent>(NetworkMetricTypes.ServerLogReceived.Id));

            sceneEvent.Sample(
                collection.GetEventValues<SceneEventMetric>(NetworkMetricTypes.SceneEventSent.Id),
                collection.GetEventValues<SceneEventMetric>(NetworkMetricTypes.SceneEventReceived.Id));

            ownershipChange.Sample(
                collection.GetEventValues<OwnershipChangeEvent>(NetworkMetricTypes.OwnershipChangeSent.Id),
                collection.GetEventValues<OwnershipChangeEvent>(NetworkMetricTypes.OwnershipChangeReceived.Id));
            
            networkMessage.Sample(
                collection.GetEventValues<NetworkMessageEvent>(NetworkMetricTypes.NetworkMessageSent.Id),
                collection.GetEventValues<NetworkMessageEvent>(NetworkMetricTypes.NetworkMessageReceived.Id));
        }
    }
}