using System;
using System.Linq;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class ThresholdSystemTests
    {
        private ThresholdSystem m_ThresholdSystem;

        [SetUp]
        public void Setup()
        {
            m_ThresholdSystem = new ThresholdSystem();
        }

        [Test]
        public void Observe_WhenNoThresholdsRegistered_ProducesSnapshotWithNoEvents()
        {
            // Arrange
            var snapshot = new MetricCollectionBuilder()
                .Build();

            m_ThresholdSystem.RegisterObserver(new TestObserver(snapshot => VerifyNoEvents(snapshot)));

            // Act
            m_ThresholdSystem.Observe(snapshot);
        }

        [Test]
        public void Observe_WhenThresholdRegisteredAndNoThresholdConditionMet_ProducesSnapshotWithNoEvents()
        {
            // Arrange
            var metricName = Guid.NewGuid().ToString();
            var snapshot = new MetricCollectionBuilder()
                .WithCounters(new Counter(metricName, 10))
                .Build();

            m_ThresholdSystem.RegisterCondition(
                metricName,
                Condition.GreaterThan(15),
                metric => { });

            m_ThresholdSystem.RegisterObserver(new TestObserver(snapshot => VerifyNoEvents(snapshot)));

            // Act
            m_ThresholdSystem.Observe(snapshot);
        }

        [Test]
        public void Observe_WhenThresholdRegisteredAndThresholdConditionIsMet_ProducesSnapshotWithThresholdEvent()
        {
            // Arrange
            var metricName = Guid.NewGuid().ToString();
            var snapshot = new MetricCollectionBuilder()
                .WithCounters(new Counter(metricName, 10))
                .Build();

            m_ThresholdSystem.RegisterCondition(
                metricName,
                Condition.GreaterThan(9),
                metric => { });
            
            m_ThresholdSystem.RegisterObserver(new TestObserver(snapshot => VerifySingleThresholdEvent(snapshot, metricName)));

            // Act
            m_ThresholdSystem.Observe(snapshot);
        }

        [Test]
        public void Observe_WhenSimpleThresholdRegisteredAndConditionIsNotMet_DoesNotInvokeCallback()
        {
            // Arrange
            var metricName = Guid.NewGuid().ToString();
            var snapshot = new MetricCollectionBuilder()
                .WithCounters(new Counter(metricName, 10))
                .Build();

            var callbackInvoked = false;
            m_ThresholdSystem.RegisterCondition(
                metricName,
                Condition.GreaterThan(10),
                metric =>
                {
                    callbackInvoked = true;
                });

            // Act
            m_ThresholdSystem.Observe(snapshot);

            // Assert
            Assert.IsFalse(callbackInvoked);
        }

        [Test]
        public void Observe_WhenSimpleThresholdRegisteredAndConditionIsMet_InvokesCallback()
        {
            // Arrange
            var metricName = Guid.NewGuid().ToString();
            var snapshot = new MetricCollectionBuilder()
                .WithCounters(new Counter(metricName, 10))
                .Build();

            var callbackInvoked = false;
            m_ThresholdSystem.RegisterCondition(
                metricName,
                Condition.GreaterThan(9),
                metric =>
                {
                    callbackInvoked = true;
                });

            // Act
            m_ThresholdSystem.Observe(snapshot);

            // Assert
            Assert.IsTrue(callbackInvoked);
        }

        [Test]
        public void Observe_WhenCustomValueProviderThresholdRegisteredAndConditionIsNotMet_DoesNotInvokeCallback()
        {
            // Arrange
            var metricName = Guid.NewGuid().ToString();
            var snapshot = new MetricCollectionBuilder()
                .WithCounters(new Counter(metricName, 11))
                .Build();

            var callbackInvoked = false;
            m_ThresholdSystem.RegisterCondition(
                metricName,
                (IMetric<long> counter) => counter.Value % 2,
                Condition.EqualTo(0),
                metric =>
                {
                    callbackInvoked = true;
                });

            // Act
            m_ThresholdSystem.Observe(snapshot);

            // Assert
            Assert.IsFalse(callbackInvoked);
        }

        [Test]
        public void Observe_WhenCustomValueProviderThresholdRegisteredAndConditionIsMet_InvokesCallback()
        {
            // Arrange
            var metricName = Guid.NewGuid().ToString();
            var snapshot = new MetricCollectionBuilder()
                .WithCounters(new Counter(metricName, 10))
                .Build();

            var callbackInvoked = false;
            m_ThresholdSystem.RegisterCondition(
                metricName,
                (IMetric<long> counter) => counter.Value % 2,
                Condition.EqualTo(0),
                metric =>
                {
                    callbackInvoked = true;
                });

            // Act
            m_ThresholdSystem.Observe(snapshot);

            // Assert
            Assert.IsTrue(callbackInvoked);
        }

        [Test]
        public void Observe_WhenMultipleThresholdRegistered_InvokesCallbacksForCorrectThresholds()
        {
            // Arrange
            var counterMetricName = Guid.NewGuid().ToString();
            var gaugeMetricName = Guid.NewGuid().ToString();
            var snapshot = new MetricCollectionBuilder()
                .WithCounters(new Counter(counterMetricName, 10))
                .WithGauges(new Gauge(gaugeMetricName, 10.0d))
                .Build();

            var counterCallbackInvoked = false;
            m_ThresholdSystem.RegisterCondition(
                counterMetricName,
                Condition.GreaterThan(10),
                metric =>
                {
                    counterCallbackInvoked = true;
                });

            var gaugeCallbackInvoked = false;
            m_ThresholdSystem.RegisterCondition(
                gaugeMetricName,
                Condition.GreaterThan(5.0d),
                metric =>
                {
                    gaugeCallbackInvoked = true;
                });

            // Act
            m_ThresholdSystem.Observe(snapshot);

            // Assert
            Assert.IsFalse(counterCallbackInvoked);
            Assert.IsTrue(gaugeCallbackInvoked);
        }

        private bool VerifyNoEvents(MetricCollection collection)
        {
            var thresholdEventsMetric = collection.Metrics.SingleOrDefault(x => x.Name == "Threshold Events");
            Assert.NotNull(thresholdEventsMetric);

            var typedMetric = thresholdEventsMetric as IEventMetric<ThresholdAlert>;
            Assert.NotNull(typedMetric);
            Assert.IsEmpty(typedMetric.Values);

            return true;
        }

        private bool VerifySingleThresholdEvent(MetricCollection collection, string metricName)
        {
            var thresholdEventsMetric = collection.Metrics.SingleOrDefault(x => x.Name == "Threshold Events");
            Assert.NotNull(thresholdEventsMetric);

            var typedMetric = thresholdEventsMetric as IEventMetric<ThresholdAlert>;
            Assert.NotNull(typedMetric);
            Assert.IsNotEmpty(typedMetric.Values);

            var foundMetric = typedMetric.Values.First();
            Assert.AreEqual(metricName, foundMetric.Metric.Name);

            return true;
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