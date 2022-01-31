using System;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class GaugeTests
    {
        [Test]
        public void Set_Always_SetsUnderlyingValueToSpecifiedAmount()
        {
            // Arrange
            var gauge = new Gauge(Guid.NewGuid().ToString());
            var value = new Random().NextDouble();

            // Act
            gauge.Set(value);

            // Assert
            Assert.AreEqual(value, gauge.Value);
        }
    }
}