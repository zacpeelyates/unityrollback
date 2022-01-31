using System;
using System.Linq;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class MetricEventTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Constructor_WhenNameIsInvalid_ThrowsException(string name)
        {
            Assert.Throws<ArgumentNullException>(() => new EventMetric(name));
        }

        [Test]
        public void ResetStringMetricEvent_Always_ClearsUnderlyingCollection()
        {
            // Arrange
            var metric = new EventMetric(Guid.NewGuid().ToString());
            metric.Mark(Guid.NewGuid().ToString());

            // Act
            metric.Reset();

            // Assert
            Assert.IsEmpty(metric.Values);
        }

        [Test]
        public void MarkStringMetricEvent_Always_AddsValueToUnderlyingCollection()
        {
            // Arrange
            var metric = new EventMetric(Guid.NewGuid().ToString());
            var value = Guid.NewGuid().ToString();

            // Act
            metric.Mark(value);

            // Assert
            Assert.AreEqual(1, metric.Values.Count);
            Assert.AreEqual(value, metric.Values.FirstOrDefault());
        }

        [Test]
        public void MarkStringMetricEvent_WithExistingItemsInCollection_KeepsUnderlyingItemsInOrder()
        {
            // Arrange
            var metric = new EventMetric(Guid.NewGuid().ToString());
            var values = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            // Act
            foreach (var value in values)
            {
                metric.Mark(value);
            }

            // Assert
            CollectionAssert.AreEquivalent(values, metric.Values);
        }

        [Test]
        public void ResetCustomStructMetricEvent_Always_ClearsUnderlyingCollection()
        {
            // Arrange
            var metric = new EventMetric<CustomEvent>(Guid.NewGuid().ToString());
            metric.Mark(new CustomEvent());

            // Act
            metric.Reset();

            // Assert
            Assert.IsEmpty(metric.Values);
        }

        [Test]
        public void MarkCustomStructMetricEvent_Always_AddsValueToUnderlyingCollection()
        {
            // Arrange
            var metric = new EventMetric<CustomEvent>(Guid.NewGuid().ToString());
            var value = new CustomEvent(Guid.NewGuid().ToString());

            // Act
            metric.Mark(value);

            // Assert
            Assert.AreEqual(1, metric.Values.Count);
            Assert.AreEqual(value, metric.Values.FirstOrDefault());
        }

        [Test]
        public void MarkCustomStructMetricEvent_WithExistingItemsInCollection_KeepsUnderlyingItemsInOrder()
        {
            // Arrange
            var metric = new EventMetric<CustomEvent>(Guid.NewGuid().ToString());
            var values = new[] { new CustomEvent(Guid.NewGuid().ToString()), new CustomEvent(Guid.NewGuid().ToString()) };

            // Act
            foreach (var value in values)
            {
                metric.Mark(value);
            }

            // Assert
            CollectionAssert.AreEquivalent(values, metric.Values);
        }

        [Serializable]
        public struct CustomEvent
        {
            public CustomEvent(string id)
            {
                Id = id;
            }

            public string Id { get; }
        }
    }
}