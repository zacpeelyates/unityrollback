using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;
using UnityEditor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class TreeModelBuilder
    {
        TreeModel m_Tree;
        Dictionary<ulong, TreeModelNode> m_ConnectionsById = new Dictionary<ulong, TreeModelNode>();
        Dictionary<ulong, Dictionary<ulong, TreeModelNode>> m_NetworkObjectsByConnectionsById = new Dictionary<ulong, Dictionary<ulong, TreeModelNode>>();
        MetricCollection m_MetricCollection;

        public TreeModelBuilder(MetricCollection metricCollection)
        {
            m_MetricCollection = metricCollection;
            m_Tree = new TreeModel();
        }

        public TreeModel Build()
        {
            return m_Tree;
        }
        
        internal TreeModelBuilder AddUnderConnection<TMetricType, TViewModelType>(
            DirectionalMetricInfo sentMetric,
            DirectionalMetricInfo receivedMetric,
            Func<TMetricType, TreeModelNode, TViewModelType> viewModelFactory,
            Func<TMetricType, bool> filter = null)
            where TMetricType : struct, INetworkMetricEvent
            where TViewModelType : ViewModelBase
        {
            if (viewModelFactory == null)
            {
                throw new ArgumentNullException(nameof(viewModelFactory));
            }

            if (m_MetricCollection.TryGetEvent<TMetricType>(sentMetric.Id, out var sentEvent))
            {
                foreach (var value in sentEvent.Values)
                {
                    if (filter != null && !filter.Invoke(value))
                    {
                        continue;
                    }
                    var connection = GetOrCreateConnection(value.Connection);
                    var viewModel = viewModelFactory.Invoke(value, connection);
                    viewModel.Bytes = new BytesSentAndReceived(sent: value.BytesCount);
                    connection.AddChild(new TreeModelNode(viewModel));
                    AddBytesToParentNodes(connection, viewModel.Bytes);
                }
            }
            
            if (m_MetricCollection.TryGetEvent<TMetricType>(receivedMetric.Id, out var receivedEvent))
            {
                foreach (var value in receivedEvent.Values)
                {
                    if (filter != null && !filter.Invoke(value))
                    {
                        continue;
                    }
                    var connection = GetOrCreateConnection(value.Connection);
                    var viewModel = viewModelFactory.Invoke(value, connection);
                    viewModel.Bytes = new BytesSentAndReceived(received: value.BytesCount);
                    connection.AddChild(new TreeModelNode(viewModel));
                    AddBytesToParentNodes(connection, viewModel.Bytes);
                }
            }

            return this;
        }
        
        internal TreeModelBuilder AddUnderNetworkObject<TMetricType, TViewModelType>(
            DirectionalMetricInfo sentMetric,
            DirectionalMetricInfo receivedMetric, 
            Func<TMetricType, TreeModelNode, TViewModelType> viewModelFactory) 
            where TMetricType : struct, INetworkObjectEvent, INetworkMetricEvent
            where TViewModelType : ViewModelBase
        {
            if (viewModelFactory == null)
            {
                throw new ArgumentNullException(nameof(viewModelFactory));
            }
            
            if (m_MetricCollection.TryGetEvent<TMetricType>(sentMetric.Id, out var sentEvent))
            {
                foreach (var value in sentEvent.Values)
                {
                    var gameObject = GetOrCreateGameObject(value.Connection, value.NetworkId);
                    var viewModel = viewModelFactory.Invoke(value, gameObject);
                    viewModel.Bytes = new BytesSentAndReceived(sent: value.BytesCount);
                    gameObject.AddChild(new TreeModelNode(viewModel));
                    AddBytesToParentNodes(gameObject, viewModel.Bytes);
                }
            }
            
            if (m_MetricCollection.TryGetEvent<TMetricType>(receivedMetric.Id, out var receivedEvent))
            {
                foreach (var value in receivedEvent.Values)
                {
                    var gameObject = GetOrCreateGameObject(value.Connection, value.NetworkId);
                    var viewModel = viewModelFactory.Invoke(value, gameObject);
                    viewModel.Bytes = new BytesSentAndReceived(received: value.BytesCount);
                    gameObject.AddChild(new TreeModelNode(viewModel));
                    AddBytesToParentNodes(gameObject, viewModel.Bytes);
                }
            }

            return this;
        }
        
        TreeModelNode GetOrCreateConnection(ConnectionInfo connectionInfo)
        {
            if (!m_ConnectionsById.TryGetValue(connectionInfo.Id, out var node))
            {
                var localConnection = new ConnectionInfo(m_MetricCollection.ConnectionId);
                node = new TreeModelNode(new ConnectionViewModel(connectionInfo, localConnection));
                m_ConnectionsById[connectionInfo.Id] = node;
                m_NetworkObjectsByConnectionsById[connectionInfo.Id] = new Dictionary<ulong, TreeModelNode>();
                m_Tree.AddChild(node);
            }

            return node;
        }

        TreeModelNode GetOrCreateGameObject(ConnectionInfo connectionInfo, NetworkObjectIdentifier networkObjectIdentifier)
        {
            var connectionNode = GetOrCreateConnection(connectionInfo);
            var networkObjectLookup = m_NetworkObjectsByConnectionsById[connectionInfo.Id];
            if (!networkObjectLookup.TryGetValue(networkObjectIdentifier.NetworkId, out var node))
            {
                node = new TreeModelNode(new GameObjectViewModel(
                    networkObjectIdentifier,
                    connectionNode.RowData,
                    () =>
                    {
                        var foundNetworkObject =
                            NetworkSolutionInterface.NetworkObjectProvider.GetNetworkObject(
                                networkObjectIdentifier.NetworkId);
                        if (foundNetworkObject)
                        {
                            EditorGUIUtility.PingObject(foundNetworkObject);
                        }
                    }));
                connectionNode.AddChild(node);
                networkObjectLookup[networkObjectIdentifier.NetworkId] = node;
            }

            return node;
        }

        static void AddBytesToParentNodes(TreeModelNode node, BytesSentAndReceived bytes)
        {
            var rowData = (ViewModelBase)node.RowData;
            rowData.Bytes += bytes;
            if (node.Parent != null)
            {
                // Note - recursion
                AddBytesToParentNodes(node.Parent, bytes);
            }
        }
    }
}
