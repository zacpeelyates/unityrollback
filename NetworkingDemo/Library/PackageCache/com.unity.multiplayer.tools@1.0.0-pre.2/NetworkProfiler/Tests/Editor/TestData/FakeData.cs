using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStats.Tests;
using Unity.Multiplayer.Tools.NetworkProfiler.Editor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    static class FakeData
    {
        static EventMetric<T> Events<T>(string name, T[] events)
            where T : struct
        {
            var metric = new EventMetric<T>(name);
            foreach (var evt in events)
            {
                metric.Mark(evt);
            }

            return metric;
        }
        
        static readonly ConnectionInfo s_Connection1 = new ConnectionInfo(1);
        static readonly ConnectionInfo s_Connection2 = new ConnectionInfo(2);
        static readonly NetworkObjectIdentifier s_Object1 = new NetworkObjectIdentifier("Bullet", 101);
        static readonly NetworkObjectIdentifier s_Object2 = new NetworkObjectIdentifier(
            "Tombstone with a really long and inconvenient name. It should overflow in the UI rather than overlapping other text! It needs to be very long indeed to trigger this behavior in the tree view, which provides a lot of space for the names along the left hand side.", 
            102);
        
        public static readonly TreeModel SingleFullConnections = TreeModelUtility.CreateActivityTreeStructure(
            MetricCollectionTestUtility.ConstructFromMetrics(localConnectionId: 1,  metrics: new List<IMetric>
            {
                Events(NetworkMetricTypes.ObjectSpawnedReceived.Id, new []
                {
                    new ObjectSpawnedEvent(s_Connection1, s_Object1, 1),
                }),
                
                Events(NetworkMetricTypes.ObjectDestroyedReceived.Id, new []
                {
                    new ObjectDestroyedEvent(s_Connection1, s_Object1, 1), 
                }),
                
                Events(NetworkMetricTypes.OwnershipChangeReceived.Id, new []
                {
                    new OwnershipChangeEvent(s_Connection1, s_Object1, 2), 
                }),
                
                Events(NetworkMetricTypes.RpcReceived.Id, new []
                {
                    new RpcEvent(s_Connection1, s_Object1, "Shoot", "BulletComponent", 1), 
                    new RpcEvent(s_Connection1, s_Object1, "Shoot", "BulletComponent", 1), 
                }),
                
                Events(NetworkMetricTypes.NetworkVariableDeltaReceived.Id, new []
                {
                    new NetworkVariableEvent(s_Connection1, s_Object1, "Distance", "BulletComponent", 1), 
                }),
                
                Events(NetworkMetricTypes.NamedMessageReceived.Id, new []
                {
                    new NamedMessageEvent(s_Connection1, "Named Message", 1), 
                }),
                
                Events(NetworkMetricTypes.UnnamedMessageReceived.Id, new []
                {
                    new UnnamedMessageEvent(s_Connection1, 1), 
                }),
                
                Events(NetworkMetricTypes.ServerLogSent.Id, new []
                {
                    new ServerLogEvent(s_Connection1, LogLevel.Info, 1), 
                }),
                
                Events(NetworkMetricTypes.SceneEventSent.Id, new []
                {
                    new SceneEventMetric(s_Connection1, "Load", "myScene", 1), 
                })
            }));

        public static readonly TreeModel FullSetOfConnections = TreeModelUtility.CreateActivityTreeStructure(
            MetricCollectionTestUtility.ConstructFromMetrics(localConnectionId: 1,  metrics: new List<IMetric>
            {
                Events(NetworkMetricTypes.ObjectSpawnedReceived.Id, new []
                {
                    new ObjectSpawnedEvent(s_Connection1, s_Object1, 1), 
                    new ObjectSpawnedEvent(s_Connection1, s_Object2, 1), 
                }),
                
                Events(NetworkMetricTypes.ObjectDestroyedReceived.Id, new []
                {
                    new ObjectDestroyedEvent(s_Connection1, s_Object1, 1), 
                    
                    new ObjectDestroyedEvent(s_Connection2, s_Object1, 1), 
                    new ObjectDestroyedEvent(s_Connection2, s_Object2, 1), 
                }),
                
                Events(NetworkMetricTypes.OwnershipChangeReceived.Id, new []
                {
                    new OwnershipChangeEvent(s_Connection1, s_Object1, 2),
                    
                    new OwnershipChangeEvent(s_Connection2, s_Object1, 1),
                    new OwnershipChangeEvent(s_Connection2, s_Object2, 1),
                }),
                
                Events(NetworkMetricTypes.RpcReceived.Id, new []
                {
                    new RpcEvent(s_Connection1, s_Object1, "Shoot", "BulletComponent", 1), 
                    new RpcEvent(s_Connection1, s_Object1, "Shoot", "BulletComponent", 1),
                    
                    new RpcEvent(s_Connection2, s_Object1, "Shoot", "BulletComponent", 1),
                    new RpcEvent(s_Connection2, s_Object2, "Shoot", "BulletComponent", 1),
                }),
                
                Events(NetworkMetricTypes.NetworkVariableDeltaReceived.Id, new []
                {
                    new NetworkVariableEvent(s_Connection1, s_Object1, "Distance", "BulletComponent", 1),
                    
                    new NetworkVariableEvent(s_Connection2, s_Object1, "Distance", "BulletComponent", 1),
                    new NetworkVariableEvent(s_Connection2, s_Object2, "Distance", "BulletComponent", 1),
                }),
                
                Events(NetworkMetricTypes.NamedMessageReceived.Id, new []
                {
                    new NamedMessageEvent(s_Connection1, "Named Message", 1),
                    
                    new NamedMessageEvent(s_Connection2, "Named Message", 1),
                }),
                
                Events(NetworkMetricTypes.UnnamedMessageReceived.Id, new []
                {
                    new UnnamedMessageEvent(s_Connection1, 1),
                    
                    new UnnamedMessageEvent(s_Connection2, 1),
                }),
                
                Events(NetworkMetricTypes.ServerLogSent.Id, new []
                {
                    new ServerLogEvent(s_Connection1, LogLevel.Info, 1),
                    
                    new ServerLogEvent(s_Connection2, LogLevel.Info, 1),
                }),
                
                Events(NetworkMetricTypes.SceneEventSent.Id, new []
                {
                    new SceneEventMetric(s_Connection1, "Load", "myScene", 1),
                    
                    new SceneEventMetric(s_Connection2, "Load", "myScene", 1)
                })
            }));
    }
}