using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class ConditionTests
    {
        [TestCase(1, 1, true)]
        [TestCase(2, 1, false)]
        [TestCase(1, 2, false)]
        public void LongEqualTo_Always_ReturnsExpectedResult(long currentValue, long threshold, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Condition.EqualTo(threshold).Invoke(currentValue));
        }

        [TestCase(1, 1, false)]
        [TestCase(2, 1, false)]
        [TestCase(1, 2, true)]
        public void LongLessThan_Always_ReturnsExpectedResult(long currentValue, long threshold, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Condition.LessThan(threshold).Invoke(currentValue));
        }

        [TestCase(1, 1, true)]
        [TestCase(2, 1, false)]
        [TestCase(1, 2, true)]
        public void LongLessThanOrEqualTo_Always_ReturnsExpectedResult(long currentValue, long threshold, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Condition.LessThanOrEqualTo(threshold).Invoke(currentValue));
        }

        [TestCase(1, 1, false)]
        [TestCase(2, 1, true)]
        [TestCase(1, 2, false)]
        public void LongGreaterThan_Always_ReturnsExpectedResult(long currentValue, long threshold, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Condition.GreaterThan(threshold).Invoke(currentValue));
        }

        [TestCase(1, 1, true)]
        [TestCase(2, 1, true)]
        [TestCase(1, 2, false)]
        public void LongGreaterThanOrEqualTo_Always_ReturnsExpectedResult(long currentValue, long threshold, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Condition.GreaterThanOrEqualTo(threshold).Invoke(currentValue));
        }

        [TestCase(1.0d, 1.0d, false)]
        [TestCase(2.0d, 1.0d, false)]
        [TestCase(1.0d, 2.0d, true)]
        public void DoubleLessThan_Always_ReturnsExpectedResult(double currentValue, double threshold, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Condition.LessThan(threshold).Invoke(currentValue));
        }

        [TestCase(1.0d, 1.0d, false)]
        [TestCase(2.0d, 1.0d, true)]
        [TestCase(1.0d, 2.0d, false)]
        public void DoubleGreaterThan_Always_ReturnsExpectedResult(double currentValue, double threshold, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Condition.GreaterThan(threshold).Invoke(currentValue));
        }
    }
}