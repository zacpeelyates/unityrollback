using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStats.Tests;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class TreeModelBuilderTests
    {
        static EventMetric<T> Events<T>(string name, T[] events) where T : struct
        {
            var metric = new EventMetric<T>(name);
            foreach (var evt in events)
            {
                metric.Mark(evt);
            }

            return metric;
        }

        readonly struct TestMetric : INetworkObjectEvent, INetworkMetricEvent
        {
            public NetworkObjectIdentifier NetworkId { get; }
            public ConnectionInfo Connection { get; }
            public long BytesCount { get; }

            public TestMetric(ConnectionInfo connectionInfo, long bytes) : this(connectionInfo, default, bytes){}
            
            public TestMetric(ConnectionInfo connectionInfo, NetworkObjectIdentifier objectIdentifier, long bytes)
            {
                Connection = connectionInfo;
                BytesCount = bytes;
                NetworkId = objectIdentifier;
            }
        }

        class TestView : ViewModelBase
        {
            public static Func<TestMetric, TreeModelNode, TestView> ObjectNameAsName =
                (metric, parent) => new TestView(parent.RowData, metric.NetworkId.Name);
            public static Func<TestMetric, TreeModelNode, TestView> ConnectionIdAsName =
                (metric, parent) => new TestView(parent.RowData, metric.Connection.Id.ToString());
            TestView(IRowData parent, string name) : base(parent, name, name, name, null) { }
        }

        static readonly DirectionalMetricInfo k_RpcSent = new DirectionalMetricInfo(MetricType.Rpc, NetworkDirection.Sent);
        static readonly DirectionalMetricInfo k_RpcReceived = new DirectionalMetricInfo(MetricType.Rpc, NetworkDirection.Received);

        static readonly ConnectionInfo k_ConnectionInfo1 = new ConnectionInfo(1);
        static readonly ConnectionInfo k_ConnectionInfo2 = new ConnectionInfo(2);
        
        static readonly NetworkObjectIdentifier k_ObjectIdentifier1 = new NetworkObjectIdentifier(k_ObjectName1, k_ObjectId1);
        const string k_ObjectName1 = nameof(k_ObjectName1);
        const int k_ObjectId1 = 1;
        
        static readonly NetworkObjectIdentifier s_ObjectIdentifier2 = new NetworkObjectIdentifier(k_ObjectName2, k_ObjectId2);
        const string k_ObjectName2 = nameof(k_ObjectName2);
        const int k_ObjectId2 = 2;

        static void AssertConnectionCount(TreeModel treeModel, int count)
        {
            Assert.AreEqual(count, treeModel.Children.Count(s => s.RowData is ConnectionViewModel));
        }

        static TreeModelNode GetConnectionNode(TreeModel treeModel, int id)
        {
            return treeModel.Children
                .Where(s => s.RowData is ConnectionViewModel)
                .ToList()[id];
        }

        static void AssertGameObjectCount(TreeModelNode connectionNode, int count)
        {
            Assert.AreEqual(count, connectionNode.Children.Count(s => s.RowData is GameObjectViewModel));
        }

        static TreeModelNode GetGameObjectNode(TreeModelNode connectionNode, int id)
        {
            return connectionNode.Children
                .Where(s => s.RowData is GameObjectViewModel)
                .ToList()[id];
        }
        
        [Test]
        public void ValidateTreeStructure_OneNetworkObjectEvent()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                })
            }));

            var tree = builder.AddUnderNetworkObject(
                k_RpcSent,
                k_RpcReceived,
                TestView.ObjectNameAsName)
                .Build();
            
            // 1 connection
            AssertConnectionCount(tree, 1);
            var connectionNode = GetConnectionNode(tree, 0);
            
            // 1 gameobject
            AssertGameObjectCount(connectionNode, 1);
            var gameObjectNode = GetGameObjectNode(connectionNode, 0);
            
            // child under gameobject
            Assert.AreEqual(1, gameObjectNode.Children.Count);
            Assert.AreEqual(k_ObjectName1, ((TestView)gameObjectNode.Children[0].RowData).Name);
        }
        
        [Test]
        public void ValidateTreeStructure_TwoNetworkObjectEventUnderSameObject()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                })
            }));

            var tree = builder.AddUnderNetworkObject(
                k_RpcSent,
                k_RpcReceived,
                TestView.ObjectNameAsName)
                .Build();
            
            // 1 connection
            AssertConnectionCount(tree, 1);
            var connectionNode = GetConnectionNode(tree, 0);
            
            // 1 gameobject
            AssertGameObjectCount(connectionNode, 1);
            var gameObjectNode = GetGameObjectNode(connectionNode, 0);
            
            // child under gameobject
            Assert.AreEqual(2, gameObjectNode.Children.Count);
            Assert.AreEqual(k_ObjectName1, ((TestView)gameObjectNode.Children[0].RowData).Name);
            Assert.AreEqual(k_ObjectName1, ((TestView)gameObjectNode.Children[1].RowData).Name);
        }
        
        [Test]
        public void ValidateTreeStructure_TwoNetworkObjectEventUnderDifferentObjects()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                    new TestMetric(k_ConnectionInfo1, s_ObjectIdentifier2, 1),
                })
            }));

            var tree = builder.AddUnderNetworkObject(
                k_RpcSent,
                k_RpcReceived,
                TestView.ObjectNameAsName)
                .Build();
            
            // 1 connection
            AssertConnectionCount(tree, 1);
            var connectionNode = GetConnectionNode(tree, 0);
            
            // 2 gameobjects
            AssertGameObjectCount(connectionNode, 2);
            
            { // game object 1
                var gameObjectNode1 = GetGameObjectNode(connectionNode, 0);

                Assert.AreEqual(1, gameObjectNode1.Children.Count);
                Assert.AreEqual(k_ObjectName1, ((TestView)gameObjectNode1.Children[0].RowData).Name);
                
                // game object formatted correctly
                var gameObjectView = (GameObjectViewModel)gameObjectNode1.RowData;
                var sampleView = new GameObjectViewModel(k_ObjectIdentifier1, connectionNode.RowData);
                Assert.AreEqual(gameObjectView.Name, sampleView.Name);
            }
            
            { // game object 2
                var gameObjectNode2 = GetGameObjectNode(connectionNode, 1);
                Assert.AreEqual(1, gameObjectNode2.Children.Count);
                Assert.AreEqual(k_ObjectName2, ((TestView)gameObjectNode2.Children[0].RowData).Name);
                
                // game object formatted correctly
                var gameObjectView = (GameObjectViewModel)gameObjectNode2.RowData;
                var sampleView = new GameObjectViewModel(s_ObjectIdentifier2, connectionNode.RowData);
                Assert.AreEqual(gameObjectView.Name, sampleView.Name);
            }
        }
        
        [Test]
        public void ValidateTreeStructure_TwoNetworkObjectEventUnderDifferentConnections()
        {
            var metricCollection = MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                    new TestMetric(k_ConnectionInfo2, k_ObjectIdentifier1, 1),
                })
            });
            metricCollection.ConnectionId = 0;
            var localConnection = new ConnectionInfo(metricCollection.ConnectionId);
            var builder = new TreeModelBuilder(metricCollection);

            var tree = builder.AddUnderNetworkObject(
                k_RpcSent,
                k_RpcReceived,
                TestView.ObjectNameAsName)
                .Build();
            
            // 2 connections
            AssertConnectionCount(tree, 2);
            {
                var connectionNode = GetConnectionNode(tree, 0);

                // connection node formatted correctly
                var connectionView = (ConnectionViewModel)connectionNode.RowData;
                var sampleView = new ConnectionViewModel(k_ConnectionInfo1, localConnection);
                Assert.AreEqual(connectionView.Name, sampleView.Name);
                
                // 1 gameobject
                AssertGameObjectCount(connectionNode, 1);
                var gameObjectNode = GetGameObjectNode(connectionNode, 0);

                Assert.AreEqual(1, gameObjectNode.Children.Count);
                Assert.AreEqual(k_ObjectName1, ((TestView)gameObjectNode.Children[0].RowData).Name);
            }
            {
                var connectionNode = GetConnectionNode(tree, 1);

                // connection node formatted correctly
                var connectionView = (ConnectionViewModel)connectionNode.RowData;
                var sampleView = new ConnectionViewModel(k_ConnectionInfo2, localConnection);
                Assert.AreEqual(connectionView.Name, sampleView.Name);
                
                // 1 gameobject
                AssertGameObjectCount(connectionNode, 1);
                var gameObjectNode = GetGameObjectNode(connectionNode, 0);

                Assert.AreEqual(1, gameObjectNode.Children.Count);
                Assert.AreEqual(k_ObjectName1, ((TestView)gameObjectNode.Children[0].RowData).Name);
            }
        }
        
        [Test]
        public void ValidateTreeStructure_OneEventUnderSameConnection()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                })
            }));

            var tree = builder.AddUnderConnection(
                k_RpcSent,
                k_RpcReceived,
                TestView.ConnectionIdAsName)
                .Build();
            
            // 1 connection
            AssertConnectionCount(tree, 1);
            var connectionNode = GetConnectionNode(tree, 0);

            // child under connection
            Assert.AreEqual(1, connectionNode.Children.Count);
            Assert.AreEqual(k_ConnectionInfo1.Id.ToString(), ((TestView)connectionNode.Children[0].RowData).Name);
        }
        
        [Test]
        public void ValidateTreeStructure_TwoEventUnderSameConnection()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                })
            }));

            var tree = builder.AddUnderConnection(
                    k_RpcSent,
                    k_RpcReceived,
                    TestView.ConnectionIdAsName)
                .Build();
            
            // 1 connection
            AssertConnectionCount(tree, 1);
            var connectionNode = GetConnectionNode(tree, 0);
            
            // child under connection
            Assert.AreEqual(2, connectionNode.Children.Count);
            Assert.AreEqual(k_ConnectionInfo1.Id.ToString(), ((TestView)connectionNode.Children[0].RowData).Name);
            Assert.AreEqual(k_ConnectionInfo1.Id.ToString(), ((TestView)connectionNode.Children[1].RowData).Name);
        }
        
        [Test]
        public void ValidateTreeStructure_TwoEventUnderDifferentConnections()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 1),
                    new TestMetric(k_ConnectionInfo2, k_ObjectIdentifier1, 1),
                })
            }));

            var tree = builder.AddUnderConnection(
                    k_RpcSent,
                    k_RpcReceived,
                    TestView.ConnectionIdAsName)
                .Build();
            
            // 2 connections
            AssertConnectionCount(tree, 2);
            {
                var connectionNode = GetConnectionNode(tree, 0);

                // child under connection
                Assert.AreEqual(1, connectionNode.Children.Count);
                Assert.AreEqual(k_ConnectionInfo1.Id.ToString(), ((TestView)connectionNode.Children[0].RowData).Name);
            }
            {
                var connectionNode = GetConnectionNode(tree, 1);

                // child under connection
                Assert.AreEqual(1, connectionNode.Children.Count);
                Assert.AreEqual(k_ConnectionInfo2.Id.ToString(), ((TestView)connectionNode.Children[0].RowData).Name);
            }
        }
        
        [Test]
        public void ValidateGameObjectByteCounts_TwoSendEventsUnderSameGameObject()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 10),
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 5),
                })
            }));

            var tree = builder.AddUnderNetworkObject(
                    k_RpcSent,
                    k_RpcReceived,
                    TestView.ObjectNameAsName)
                .Build();
            
            AssertConnectionCount(tree, 1);
            var connection = GetConnectionNode(tree, 0);
            
            AssertGameObjectCount(connection, 1);
            var gameObject = GetGameObjectNode(connection, 0);

            var gameObjectView = (GameObjectViewModel)gameObject.RowData;
            Assert.AreEqual(15, gameObjectView.Bytes.Sent);
        }
        
        [Test]
        public void ValidateGameObjectByteCounts_SendAndReceiveUnderSameGameObject()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 10),
                    
                }),
                Events(k_RpcReceived.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 5),
                })
            }));

            var tree = builder.AddUnderNetworkObject(
                    k_RpcSent,
                    k_RpcReceived,
                    TestView.ObjectNameAsName)
                .Build();
            
            AssertConnectionCount(tree, 1);
            var connection = GetConnectionNode(tree, 0);

            AssertGameObjectCount(connection, 1);
            var gameObject = GetGameObjectNode(connection, 0);

            var gameObjectView = (GameObjectViewModel)gameObject.RowData;
            Assert.AreEqual(10, gameObjectView.Bytes.Sent);
            Assert.AreEqual(5, gameObjectView.Bytes.Received);
        }
        
        [Test]
        public void ValidateConnectionByteCounts_SendAndReceiveUnderMultipleGameObjects()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 10),
                    new TestMetric(k_ConnectionInfo1, s_ObjectIdentifier2, 20),
                    
                }),
                Events(k_RpcReceived.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 5),
                    new TestMetric(k_ConnectionInfo1, s_ObjectIdentifier2, 10),
                })
            }));

            var tree = builder.AddUnderNetworkObject(
                    k_RpcSent,
                    k_RpcReceived,
                    TestView.ObjectNameAsName)
                .Build();
            
            AssertConnectionCount(tree, 1);
            var connection = GetConnectionNode(tree, 0);
            var connectionView = (ConnectionViewModel)connection.RowData;

            Assert.AreEqual(30, connectionView.Bytes.Sent);
            Assert.AreEqual(15, connectionView.Bytes.Received);
        }
        
        [Test]
        public void ValidateConnectionByteCounts_SendAndReceiveUnderConnection()
        {
            var builder = new TreeModelBuilder(MetricCollectionTestUtility.ConstructFromMetrics(new List<IMetric>
            {
                Events(k_RpcSent.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 10),
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 20),
                    
                }),
                Events(k_RpcReceived.Id, new[]
                {
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 5),
                    new TestMetric(k_ConnectionInfo1, k_ObjectIdentifier1, 10),
                })
            }));

            var tree = builder.AddUnderConnection(
                    k_RpcSent,
                    k_RpcReceived,
                    TestView.ObjectNameAsName)
                .Build();
            
            AssertConnectionCount(tree, 1);
            var connection = GetConnectionNode(tree, 0);
            var connectionView = (ConnectionViewModel)connection.RowData;

            Assert.AreEqual(30, connectionView.Bytes.Sent);
            Assert.AreEqual(15, connectionView.Bytes.Received);
        }
    }
}
