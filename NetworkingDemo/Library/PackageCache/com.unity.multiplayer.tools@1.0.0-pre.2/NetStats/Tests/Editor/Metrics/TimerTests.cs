using System;
using System.Threading;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class TimerTests
    {
        [Test]
        public void Set_Always_SetsUnderlyingValueToSpecifiedTimeSpan()
        {
            // Arrange
            var timer = new Timer(Guid.NewGuid().ToString());
            var duration = TimeSpan.FromDays(1);

            // Act
            timer.Set(duration);

            // Assert
            Assert.AreEqual(duration, timer.Value);
        }

        [Test]
        public void Time_Always_SetsUnderlyingValueToScopeExecutionDuration()
        {
            // Arrange
            var timer = new Timer(Guid.NewGuid().ToString());

            // Act
            using (timer.Time())
            {
                Thread.Sleep(110);
            }

            // Assert
            Assert.Greater(timer.Value, TimeSpan.FromMilliseconds(100));
        }
    }
}