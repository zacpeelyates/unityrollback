using System;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class MetricTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Constructor_WhenNameIsInvalid_ThrowsException(string name)
        {
            Assert.Throws<ArgumentNullException>(() => new Counter(name));
        }

        [Test]
        public void Constructor_Always_SetsNameAndDefaultValue()
        {
            // Arrange
            var name = Guid.NewGuid().ToString();
            var value = new Random().Next();

            // Act
            var metric = new TestMetric(name, value);

            // Assert
            Assert.AreEqual(name, metric.Name);
            Assert.AreEqual(value, metric.Value);
        }

        [Test]
        public void Reset_Always_SetsUnderlyingValueToDefaultValue()
        {
            // Arrange
            var name = Guid.NewGuid().ToString();
            var value = new Random().Next();

            var metric = new TestMetric(name, value);
            metric.SetValue(100);

            // Act
            metric.Reset();

            // Assert
            Assert.AreEqual(value, metric.Value);
        }

        [Serializable]
        internal class TestMetric : Metric<int>
        {
            public TestMetric(string name, int defaultValue = default)
                : base(name, defaultValue)
            {
            }

            public void SetValue(int value)
            {
                Value = value;
            }
        }
    }
}