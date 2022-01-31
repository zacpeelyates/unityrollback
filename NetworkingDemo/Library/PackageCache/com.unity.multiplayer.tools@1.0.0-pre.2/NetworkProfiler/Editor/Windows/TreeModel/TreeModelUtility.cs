using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class TreeModelUtility
    {
        public static List<IRowData> FlattenTree(TreeModel tree)
        {
            var rowData = new List<IRowData>();
            
            foreach (var child in tree.Children)
            {
                FlattenTreeRecursive(child, rowData);
            }
            
            return rowData;
        }

        static void FlattenTreeRecursive(TreeModelNode node, List<IRowData> outList)
        {
            outList.Add(node.RowData);
            
            foreach (var child in node.Children)
            {
                FlattenTreeRecursive(child, outList);
            }
        }

        // These matches how NGO reports those message.
        // They may match the MetricTypes enum but that is coincidental. 
        static readonly IReadOnlyCollection<string> k_ExcludedNetworkMessageTypeNames = new[]
        {
            "NamedMessage",
            "UnnamedMessage",
            "SceneEventMessage",
            "ServerLogMessage"
        };

        public static TreeModel CreateMessagesTreeStructure(MetricCollection metrics)
        {
            if (metrics == null)
            {
                return new TreeModel();
            }

            return new TreeModelBuilder(metrics)
                .AddUnderConnection(
                    NetworkMetricTypes.NamedMessageSent,
                    NetworkMetricTypes.NamedMessageReceived,
                    (NamedMessageEvent metric, TreeModelNode node) 
                        => new NamedMessageEventViewModel(metric.Name, node.RowData))
                .AddUnderConnection(
                    NetworkMetricTypes.UnnamedMessageSent,
                    NetworkMetricTypes.UnnamedMessageReceived,
                    (UnnamedMessageEvent metric, TreeModelNode node) 
                        => new UnnamedMessageEventViewModel(node.RowData))
                .AddUnderConnection(
                    NetworkMetricTypes.SceneEventSent,
                    NetworkMetricTypes.SceneEventReceived,
                    (SceneEventMetric metric, TreeModelNode node)
                        => new SceneEventViewModel(metric.SceneName, metric.SceneEventType, node.RowData))
                .AddUnderConnection(
                    NetworkMetricTypes.ServerLogSent,
                    NetworkMetricTypes.ServerLogReceived,
                    (ServerLogEvent metric, TreeModelNode node) 
                        => new ServerLogEventViewModel(metric.LogLevel, node.RowData))
                .AddUnderConnection(
                    NetworkMetricTypes.NetworkMessageSent,
                    NetworkMetricTypes.NetworkMessageReceived,
                    (NetworkMessageEvent metric, TreeModelNode node)
                        => new NetworkMessageEventViewModel(metric.Name, node.RowData),
                    metric => !k_ExcludedNetworkMessageTypeNames.Contains(metric.Name))
                .Build();
        }
        
        public static TreeModel CreateActivityTreeStructure(MetricCollection metrics)
        {
            if (metrics == null)
            {
                return new TreeModel();
            }
            
            return new TreeModelBuilder(metrics)
                
                .AddUnderNetworkObject(
                    NetworkMetricTypes.ObjectSpawnedSent,
                    NetworkMetricTypes.ObjectSpawnedReceived,
                    (ObjectSpawnedEvent metric, TreeModelNode node) 
                        => new SpawnEventViewModel(metric.NetworkId, node.RowData))

                .AddUnderNetworkObject(
                    NetworkMetricTypes.ObjectDestroyedSent,
                    NetworkMetricTypes.ObjectDestroyedReceived,
                    (ObjectDestroyedEvent metric, TreeModelNode node) 
                        => new DestroyEventViewModel(node.RowData))

                .AddUnderNetworkObject(
                    NetworkMetricTypes.OwnershipChangeSent,
                    NetworkMetricTypes.OwnershipChangeReceived,
                    (OwnershipChangeEvent metric, TreeModelNode node) 
                        => new OwnershipChangeEventViewModel(node.RowData))

                .AddUnderNetworkObject(
                    NetworkMetricTypes.RpcSent,
                    NetworkMetricTypes.RpcReceived,
                    (RpcEvent metric, TreeModelNode node) 
                        => new RpcEventViewModel(metric.NetworkBehaviourName, metric.Name, node.RowData))

                .AddUnderNetworkObject(
                    NetworkMetricTypes.NetworkVariableDeltaSent,
                    NetworkMetricTypes.NetworkVariableDeltaReceived,
                    (NetworkVariableEvent metric, TreeModelNode node) 
                        => new NetworkVariableEventViewModel(metric.NetworkBehaviourName, metric.Name, node.RowData))

                .Build();
        }
    }
}
