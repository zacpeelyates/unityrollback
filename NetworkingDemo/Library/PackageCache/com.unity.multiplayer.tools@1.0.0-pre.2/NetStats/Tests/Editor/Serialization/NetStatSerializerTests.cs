using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    internal class NetStatSerializerTests
    {
        [Serializable]
        struct TestEventData
        {
            public string String1;
            public int Int1;
            public bool Bool1;

            public void AssertEquals(TestEventData other)
            {
                Assert.AreEqual(String1, other.String1);
                Assert.AreEqual(Int1, other.Int1);
                Assert.AreEqual(Bool1, other.Bool1);
            }
        }

        static MetricCollection SerializeDeserializeMetrics(List<IMetric> metrics)
        {
            var collection = MetricCollectionTestUtility.ConstructFromMetrics(metrics);
            
            var serialized = new NetStatSerializer().Serialize(collection);
            var deserialized = new NetStatSerializer().Deserialize(serialized);
            return deserialized;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenCountersInCollection_WhenSerializedDeserialized_CounterValuesCorrect(int numCounters)
        {
            var counters = new List<Counter>();
            for (int i = 0; i < numCounters; ++i)
            {
                counters.Add(new Counter("Counter"+i, i));
            }

            var result = SerializeDeserializeMetrics(counters.Cast<IMetric>().ToList());

            for (int i = 0; i < numCounters; ++i)
            {
                Assert.IsTrue(result.TryGetCounter(counters[i].Name, out var deserializedCounter));
                Assert.AreEqual(deserializedCounter.Value, counters[i].Value);
            }
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenGaugesInCollection_WhenSerializedDeserialized_GaugeValuesCorrect(int numGauges)
        {
            var gauges = new List<Gauge>();
            for (int i = 0; i < numGauges; ++i)
            {
                gauges.Add(new Gauge("Gauge"+i, i * 0.5f));
            }

            var result = SerializeDeserializeMetrics(gauges.Cast<IMetric>().ToList());

            for (int i = 0; i < numGauges; ++i)
            {
                Assert.IsTrue(result.TryGetGauge(gauges[i].Name, out var deserializedGauge));
                Assert.IsTrue(Math.Abs(deserializedGauge.Value - gauges[i].Value) < float.Epsilon);
            }
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenEventsInCollection_WhenSerializedDeserialized_EventValuesCorrect(int numEvents)
        {
            var events = new List<EventMetric>();
            for (int i = 0; i < numEvents; ++i)
            {
                var evt = new EventMetric("Event"+i);
                for (int j = 0; j < numEvents; ++j)
                {
                    evt.Mark("Mark"+j);
                }
                
                events.Add(evt);
            }

            var result = SerializeDeserializeMetrics(events.Cast<IMetric>().ToList());

            for (int i = 0; i < numEvents; ++i)
            {
                Assert.IsTrue(result.TryGetEvent(events[i].Name, out var deserializedEvent));
                Assert.AreEqual(events[i].Values.Count, deserializedEvent.Values.Count);
                Assert.IsTrue(events[i].Values.SequenceEqual(deserializedEvent.Values));
            }
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenGenericEventsInCollection_WhenSerializedDeserialized_EventValuesCorrect(int numEvents)
        {
            var events = new List<EventMetric<TestEventData>>();
            for (int i = 0; i < numEvents; ++i)
            {
                var evt = new EventMetric<TestEventData>("Event"+i);
                for (int j = 0; j < numEvents; ++j)
                {
                    evt.Mark(new TestEventData()
                    {
                        String1 = "Mark"+j,
                        Int1 = j * 10,
                        Bool1 = j % 2 == 0
                    });
                }
                
                events.Add(evt);
            }

            var result = SerializeDeserializeMetrics(events.Cast<IMetric>().ToList());

            for (int i = 0; i < numEvents; ++i)
            {
                Assert.IsTrue(result.TryGetEvent<TestEventData>(events[i].Name, out var deserializedEvent));
                Assert.AreEqual(events[i].Values.Count, deserializedEvent.Values.Count);

                var eventValues = events[i].Values.ToList();
                var deserializedEventValues = deserializedEvent.Values.ToList();
                for (int j = 0; j < eventValues.Count; ++j)
                {
                    deserializedEventValues[j].AssertEquals(eventValues[j]);
                }
            }
        }
    }
}
