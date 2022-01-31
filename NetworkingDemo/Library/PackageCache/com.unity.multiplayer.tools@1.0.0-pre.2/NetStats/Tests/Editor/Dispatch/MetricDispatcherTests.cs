using System;
using System.Linq;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class MetricDispatcherTests
    {
        private readonly string MetricName = Guid.NewGuid().ToString();

        [Test]
        public void Dispatch_WhenCounterIsRegistered_DispatchesCounterValueToObservers()
        {
            // Arrange
            var counter = new Counter(MetricName);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithCounters(counter)
                .Build();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetCounter(MetricName, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.AreEqual(10, metric.Value);
            }));

            counter.Increment(10);

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenGaugeIsRegistered_DispatchesGaugeValueToObservers()
        {
            // Arrange
            var gauge = new Gauge(MetricName);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithGauges(gauge)
                .Build();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetGauge(MetricName, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.AreEqual(10, metric.Value);
            }));

            gauge.Set(10);

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenTimerIsRegistered_DispatchesTimerValueToObservers()
        {
            // Arrange
            var timer = new Timer(MetricName);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithTimers(timer)
                .Build();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetTimer(MetricName, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.AreEqual(TimeSpan.FromHours(1), metric.Value);
            }));

            timer.Set(TimeSpan.FromHours(1));

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenEventIsRegistered_DispatchesEventValueToObservers()
        {
            // Arrange
            var metricEvent = new EventMetric(MetricName);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithMetricEvents(metricEvent)
                .Build();

            var value = Guid.NewGuid().ToString();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetEvent(MetricName, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.NotNull(metric.Values);
                Assert.AreEqual(1, metric.Values.Count);
                Assert.AreEqual(value, metric.Values.First());
            }));

            metricEvent.Mark(value);

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenTypedEventIsRegistered_DispatchesEventValueToObservers()
        {
            // Arrange
            var metricEvent = new EventMetric<TestMetricEvent>(MetricName);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithMetricEvents(metricEvent)
                .Build();

            var value = Guid.NewGuid().ToString();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetEvent<TestMetricEvent>(MetricName, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.NotNull(metric.Values);
                Assert.AreEqual(1, metric.Values.Count);
                Assert.NotNull(metric.Values.First());
                Assert.AreEqual(value, metric.Values.First().Value);
            }));

            metricEvent.Mark(new TestMetricEvent(value));

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenMetricIsResetOnDispatch_ResetsMetric()
        {
            // Arrange
            var counter = new Counter(MetricName)
            {
                ShouldResetOnDispatch = true,
            };

            var metricDispatcher = new MetricDispatcherBuilder()
                .WithCounters(counter)
                .Build();

            counter.Increment(10);

            // Act
            metricDispatcher.Dispatch();

            // Assert
            Assert.AreEqual(0, counter.Value);
        }

        [Test]
        public void Dispatch_WhenMetricIsNotResetOnDispatch_DoesNotResetMetric()
        {
            // Arrange
            var counter = new Counter(MetricName)
            {
                ShouldResetOnDispatch = false,
            };

            var metricDispatcher = new MetricDispatcherBuilder()
                .WithCounters(counter)
                .Build();

            counter.Increment(10);

            // Act
            metricDispatcher.Dispatch();

            // Assert
            Assert.AreEqual(10, counter.Value);
        }

        [Serializable]
        public struct TestMetricEvent
        {
            public TestMetricEvent(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        private class TestObserver : IMetricObserver
        {
            private readonly Action<MetricCollection> m_Assertion;

            public TestObserver(Action<MetricCollection> assertion)
            {
                m_Assertion = assertion;
            }

            public void Observe(MetricCollection collection)
            {
                m_Assertion.Invoke(collection);
            }
        }
    }
}