using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class ThresholdConfigurationTests
    {
        [Test]
        public void IsMet_WhenStatIsNotTheExpectedType_ReturnsFalse()
        {
            // Arrange
            var condition = new ThresholdConfiguration<Counter, long>(metric => metric.Value, value => value > 10);

            // Act
            var isMet = condition.IsConditionMet(new Gauge(Guid.NewGuid().ToString(), 20));

            // Assert
            Assert.IsFalse(isMet);
        }

        [Test]
        public void IsMet_WhenStatIsExpectedTypeAndConditionIsNotMet_ReturnsFalse()
        {
            // Arrange
            var condition = new ThresholdConfiguration<Counter, long>(metric => metric.Value, value => value > 10);

            // Act
            var isMet = condition.IsConditionMet(new Counter(Guid.NewGuid().ToString(), 10));

            // Assert
            Assert.IsFalse(isMet);
        }

        [Test]
        public void IsMet_WhenStatIsExpectedTypeAndConditionIsMet_ReturnsTrue()
        {
            // Arrange
            var condition = new ThresholdConfiguration<Counter, long>(metric => metric.Value, value => value > 10);

            // Act
            var isMet = condition.IsConditionMet(new Counter(Guid.NewGuid().ToString(), 20));

            // Assert
            Assert.IsTrue(isMet);
        }

        [Test]
        public void IsMet_WhenConditionThrowsException_ReturnsFalse()
        {
            // Arrange
            var condition = new ThresholdConfiguration<Counter, long>(
                metric => metric.Value,
                value => throw new Exception());

            // Act
            var isMet = condition.IsConditionMet(new Counter(Guid.NewGuid().ToString(), 10));

            // Assert
            Assert.IsFalse(isMet);
            LogAssert.Expect(LogType.Error, "Failed to evaluate threshold condition: 'Exception of type 'System.Exception' was thrown.'.");
        }
    }
}