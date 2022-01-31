using System;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class CounterTests
    {
        [Test]
        public void Increment_Always_IncrementsUnderlyingValue()
        {
            // Arrange
            var counter = new Counter(Guid.NewGuid().ToString(), 10);

            // Act
            counter.Increment();

            // Assert
            Assert.AreEqual(11, counter.Value);
        }

        [TestCase(10, 1, 11)]
        [TestCase(10, -1, 9)]
        public void Increment_Always_IncrementsUnderlyingValueBySpecifiedAmount(long initial, long increment, long expected)
        {
            // Arrange
            var counter = new Counter(Guid.NewGuid().ToString(), initial);

            // Act
            counter.Increment(increment);

            // Assert
            Assert.AreEqual(expected, counter.Value);
        }
    }
}