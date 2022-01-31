using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStats.Tests;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class ProfilerCountersTests
    {
        public struct CounterValidationParameters
        {
            internal string Case;
            internal Func<IMetric[]> Metrics;
            internal Func<ProfilerCounters, string> CounterName;
            internal long CounterValue;
            public override string ToString() => Case;
        }

        [TestCaseSource(typeof(ProfilerCountersTestData), nameof(ProfilerCountersTestData.ValidateCounterTestCases))]
        public void ValidateCounter(CounterValidationParameters parameters)
        {
            var counters = new Dictionary<string, TestCounter>();
            var factory = new CounterFactory(counters);
            var testObj = new ProfilerCounters(factory, factory);
            
            testObj.UpdateFromMetrics(
                MetricCollectionTestUtility.ConstructFromMetrics(
                    parameters.Metrics().ToList()));
            
            Assert.IsTrue(counters.TryGetValue(parameters.CounterName(testObj), out var counter));
            Assert.AreEqual(counter.value, parameters.CounterValue);
        }
        
        class TestCounter : ICounter
        {
            public long value;
            public void Sample(long inValue) => value += inValue;
        }
        
        class CounterFactory : ICounterFactory
        {
            Dictionary<string, TestCounter> m_Lookup;
            
            public CounterFactory(Dictionary<string, TestCounter> lookup)
            {
                m_Lookup = lookup;
            }
            
            public ICounter Construct(string name)
            {
                var counter = new TestCounter();
                m_Lookup.Add(name, counter);
                return counter;
            }
        }
    }
}
