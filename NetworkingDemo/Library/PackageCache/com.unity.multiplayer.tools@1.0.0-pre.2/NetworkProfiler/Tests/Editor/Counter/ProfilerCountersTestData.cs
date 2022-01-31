using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class ProfilerCountersTestData
    {
        const long k_DefaultExpectedCounterValue = 100;
        
        public static List<ProfilerCountersTests.CounterValidationParameters> ValidateCounterTestCases => new List<ProfilerCountersTests.CounterValidationParameters>()
        {
            new ProfilerCountersTests.CounterValidationParameters()
            {
                Case = NetworkMetricTypes.TotalBytesSent.DisplayName,
                Metrics = () => SingleCounterValue(NetworkMetricTypes.TotalBytesSent.Id, 100),
                CounterName = counters => counters.totalBytes.Sent,
                CounterValue = 100
            },
            new ProfilerCountersTests.CounterValidationParameters()
            {
                Case = NetworkMetricTypes.TotalBytesReceived.DisplayName,
                Metrics = () => SingleCounterValue(NetworkMetricTypes.TotalBytesReceived.Id, 100),
                CounterName = counters => counters.totalBytes.Received,
                CounterValue = 100
            },

            // RPC
            SingleEventTestCase(
                NetworkMetricTypes.RpcSent.Id, 
                new RpcEvent(default, default, default, default, k_DefaultExpectedCounterValue),
                counters => counters.rpc.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.RpcReceived.Id, 
                new RpcEvent(default, default, default, default, k_DefaultExpectedCounterValue),
                counters => counters.rpc.Bytes.Received),
            
            // Named Message
            SingleEventTestCase(
                NetworkMetricTypes.NamedMessageSent.Id, 
                new NamedMessageEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.namedMessage.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.NamedMessageReceived.Id, 
                new NamedMessageEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.namedMessage.Bytes.Received),
            
            // Unnamed Message
            SingleEventTestCase(
                NetworkMetricTypes.UnnamedMessageSent.Id, 
                new UnnamedMessageEvent(default, k_DefaultExpectedCounterValue),
                counters => counters.unnamedMessage.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.UnnamedMessageReceived.Id, 
                new UnnamedMessageEvent(default, k_DefaultExpectedCounterValue),
                counters => counters.unnamedMessage.Bytes.Received),
            
            // Network Variable Delta
            SingleEventTestCase(
                NetworkMetricTypes.NetworkVariableDeltaSent.Id, 
                new NetworkVariableEvent(default, default, default, default, k_DefaultExpectedCounterValue),
                counters => counters.networkVariableDelta.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.NetworkVariableDeltaReceived.Id, 
                new NetworkVariableEvent(default, default, default, default, k_DefaultExpectedCounterValue),
                counters => counters.networkVariableDelta.Bytes.Received),
            
            // Object Spawned
            SingleEventTestCase(
                NetworkMetricTypes.ObjectSpawnedSent.Id, 
                new ObjectSpawnedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectSpawned.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.ObjectSpawnedReceived.Id, 
                new ObjectSpawnedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectSpawned.Bytes.Received),
            
            // Object Destroyed
            SingleEventTestCase(
                NetworkMetricTypes.ObjectDestroyedSent.Id, 
                new ObjectDestroyedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectDestroyed.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.ObjectDestroyedReceived.Id, 
                new ObjectDestroyedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectDestroyed.Bytes.Received),
            
            // ServerLog
            SingleEventTestCase(
                NetworkMetricTypes.ServerLogSent.Id, 
                new ServerLogEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.serverLog.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.ServerLogReceived.Id, 
                new ServerLogEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.serverLog.Bytes.Received),
            
            // SceneEvent
            SingleEventTestCase(
                NetworkMetricTypes.SceneEventSent.Id, 
                new SceneEventMetric(default, default, default, k_DefaultExpectedCounterValue),
                counters => counters.sceneEvent.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.SceneEventReceived.Id, 
                new SceneEventMetric(default, default, default, k_DefaultExpectedCounterValue),
                counters => counters.sceneEvent.Bytes.Received),
            
            // Ownership Change
            SingleEventTestCase(
                NetworkMetricTypes.OwnershipChangeSent.Id, 
                new OwnershipChangeEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.ownershipChange.Bytes.Sent),
            SingleEventTestCase(
                NetworkMetricTypes.OwnershipChangeReceived.Id, 
                new OwnershipChangeEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.ownershipChange.Bytes.Received),
            
            // Network Message
            SingleEventTestCase(
                NetworkMetricTypes.NetworkMessageSent.Id, 
                new NetworkMessageEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.networkMessage.Events.Sent,
                1),
            SingleEventTestCase(
                NetworkMetricTypes.NetworkMessageReceived.Id, 
                new NetworkMessageEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.networkMessage.Events.Received,
                1),
            
            // Custom Message
            new ProfilerCountersTests.CounterValidationParameters()
            {
                Case = "Custom Message Sent",
                Metrics = () => new IMetric[]
                {
                    Events(NetworkMetricTypes.NamedMessageSent.Id, new[]
                    {
                        new NamedMessageEvent(default, "Test1", 100)
                    }),
                    Events(NetworkMetricTypes.UnnamedMessageSent.Id, new[]
                    {
                        new UnnamedMessageEvent(default, 100)
                    })
                },
                CounterName = counters => counters.customMessage.Bytes.Sent,
                CounterValue = 200
            },
            new ProfilerCountersTests.CounterValidationParameters()
            {
                Case = "Custom Message Received",
                Metrics = () => new IMetric[]
                {
                    Events(NetworkMetricTypes.NamedMessageReceived.Id, new[]
                    {
                        new NamedMessageEvent(default, "Test1", 100)
                    }),
                    Events(NetworkMetricTypes.UnnamedMessageReceived.Id, new[]
                    {
                        new UnnamedMessageEvent(default, 100)
                    })
                },
                CounterName = counters => counters.customMessage.Bytes.Received,
                CounterValue = 200
            },
        };

        static ProfilerCountersTests.CounterValidationParameters SingleEventTestCase<T>(
            string metricId,
            T metricEvent,
            Func<ProfilerCounters, string> counterName,
            long expectedCounterValue = k_DefaultExpectedCounterValue) where T : struct
            => new ProfilerCountersTests.CounterValidationParameters()
            {
                Case = metricId,
                Metrics = () => SingleEvent(
                    metricId,
                    metricEvent),
                CounterName = counterName,
                CounterValue = expectedCounterValue
            };
        
        static IMetric[] SingleCounterValue(string name, long value)
            => new IMetric[]
            {
                Counter(name, value)
            };

        static IMetric[] SingleEvent<T>(string name, T evt) where T : struct
            => new IMetric[]
            {
                Events(name, new[]
                {
                    evt
                })
            };

        static Counter Counter(string name, long value)
        {
            var metric = new Counter(name);
            metric.Increment(value);
            return metric;
        }
        
        static EventMetric<T> Events<T>(string name, T[] events) where T : struct
        {
            var metric = new EventMetric<T>(name);
            foreach (var evt in events)
            {
                metric.Mark(evt);
            }

            return metric;
        }
    }
}
