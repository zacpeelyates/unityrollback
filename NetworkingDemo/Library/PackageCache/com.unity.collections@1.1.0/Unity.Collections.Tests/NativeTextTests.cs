#if !UNITY_DOTSRUNTIME
using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.Text;
using Unity.Burst;
using Unity.Jobs;

namespace FixedStringTests
{

    internal class NativeTextTests
    {
        [Test]
        public void NativeTextFixedStringCtors()
        {
            using (NativeText aa = new NativeText(new FixedString32Bytes("test32"), Allocator.Temp))
            {
                Assert.True(aa != new FixedString32Bytes("test"));
                Assert.True(aa.Value == "test32");
                Assert.AreEqual("test32", aa);
            }

            using (NativeText aa = new NativeText(new FixedString64Bytes("test64"), Allocator.Temp))
            {
                Assert.True(aa != new FixedString64Bytes("test"));
                Assert.True(aa.Value == "test64");
                Assert.AreEqual("test64", aa);
            }

            using (NativeText aa = new NativeText(new FixedString128Bytes("test128"), Allocator.Temp))
            {
                Assert.True(aa != new FixedString128Bytes("test"));
                Assert.True(aa.Value == "test128");
                Assert.AreEqual("test128", aa);
            }

            using (NativeText aa = new NativeText(new FixedString512Bytes("test512"), Allocator.Temp))
            {
                Assert.True(aa != new FixedString512Bytes("test"));
                Assert.True(aa.Value == "test512");
                Assert.AreEqual("test512", aa);
            }

            using (NativeText aa = new NativeText(new FixedString4096Bytes("test4096"), Allocator.Temp))
            {
                Assert.True(aa != new FixedString4096Bytes("test"));
                Assert.True(aa.Value == "test4096");
                Assert.AreEqual("test4096", aa);
            }
        }

        [Test]
        public void NativeTextCorrectLengthAfterClear()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            Assert.True(aa.IsCreated);
            Assert.AreEqual(0, aa.Length, "Length after creation is not 0");
            aa.AssertNullTerminated();

            aa.Junk();

            aa.Clear();
            Assert.AreEqual(0, aa.Length, "Length after clear is not 0");
            aa.AssertNullTerminated();

            aa.Dispose();
        }

        [Test]
        public void NativeTextFormatExtension1Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            Assert.True(aa.IsCreated);
            aa.Junk();
            FixedString32Bytes format = "{0}";
            FixedString32Bytes arg0 = "a";
            aa.AppendFormat(format, arg0);
            aa.Add(0x61);
            Assert.AreEqual("aa", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension2Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString32Bytes format = "{0} {1}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            aa.AppendFormat(format, arg0, arg1);
            Assert.AreEqual("a b", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension3Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString32Bytes format = "{0} {1} {2}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            aa.AppendFormat(format, arg0, arg1, arg2);
            Assert.AreEqual("a b c", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension4Params()
        {
            NativeText aa = new NativeText(Allocator.Temp);
            aa.Junk();
            FixedString32Bytes format = "{0} {1} {2} {3}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            FixedString32Bytes arg3 = "d";
            aa.AppendFormat(format, arg0, arg1, arg2, arg3);
            Assert.AreEqual("a b c d", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension5Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString32Bytes format = "{0} {1} {2} {3} {4}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            FixedString32Bytes arg3 = "d";
            FixedString32Bytes arg4 = "e";
            aa.AppendFormat(format, arg0, arg1, arg2, arg3, arg4);
            Assert.AreEqual("a b c d e", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension6Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString32Bytes format = "{0} {1} {2} {3} {4} {5}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            FixedString32Bytes arg3 = "d";
            FixedString32Bytes arg4 = "e";
            FixedString32Bytes arg5 = "f";
            aa.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5);
            Assert.AreEqual("a b c d e f", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension7Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString32Bytes format = "{0} {1} {2} {3} {4} {5} {6}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            FixedString32Bytes arg3 = "d";
            FixedString32Bytes arg4 = "e";
            FixedString32Bytes arg5 = "f";
            FixedString32Bytes arg6 = "g";
            aa.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            Assert.AreEqual("a b c d e f g", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension8Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString128Bytes format = "{0} {1} {2} {3} {4} {5} {6} {7}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            FixedString32Bytes arg3 = "d";
            FixedString32Bytes arg4 = "e";
            FixedString32Bytes arg5 = "f";
            FixedString32Bytes arg6 = "g";
            FixedString32Bytes arg7 = "h";
            aa.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            Assert.AreEqual("a b c d e f g h", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension9Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString128Bytes format = "{0} {1} {2} {3} {4} {5} {6} {7} {8}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            FixedString32Bytes arg3 = "d";
            FixedString32Bytes arg4 = "e";
            FixedString32Bytes arg5 = "f";
            FixedString32Bytes arg6 = "g";
            FixedString32Bytes arg7 = "h";
            FixedString32Bytes arg8 = "i";
            aa.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            Assert.AreEqual("a b c d e f g h i", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }


        [Test]
        public void NativeTextFormatExtension10Params()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            FixedString128Bytes format = "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}";
            FixedString32Bytes arg0 = "a";
            FixedString32Bytes arg1 = "b";
            FixedString32Bytes arg2 = "c";
            FixedString32Bytes arg3 = "d";
            FixedString32Bytes arg4 = "e";
            FixedString32Bytes arg5 = "f";
            FixedString32Bytes arg6 = "g";
            FixedString32Bytes arg7 = "h";
            FixedString32Bytes arg8 = "i";
            FixedString32Bytes arg9 = "j";
            aa.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            Assert.AreEqual("a b c d e f g h i j", aa);
            aa.AssertNullTerminated();
            aa.Dispose();
        }

        [Test]
        public void NativeTextAppendGrows()
        {
            NativeText aa = new NativeText(1, Allocator.Temp);
            var origCapacity = aa.Capacity;
            for (int i = 0; i < origCapacity; ++i)
                aa.Append('a');
            Assert.AreEqual(origCapacity, aa.Capacity);
            aa.Append('b');
            Assert.GreaterOrEqual(aa.Capacity, origCapacity);
            Assert.AreEqual(new String('a', origCapacity) + "b", aa.ToString());
            aa.Dispose();
        }

        [Test]
        public void NativeTextAppendString()
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Append("aa");
            Assert.AreEqual("aa", aa.ToString());
            aa.Append("bb");
            Assert.AreEqual("aabb", aa.ToString());
            aa.Dispose();
        }


        [TestCase("Antidisestablishmentarianism")]
        [TestCase("⁣🌹🌻🌷🌿🌵🌾⁣")]
        public void NativeTextCopyFromBytesWorks(String a)
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Junk();
            var utf8 = Encoding.UTF8.GetBytes(a);
            unsafe
            {
                fixed (byte* b = utf8)
                    aa.Append(b, (ushort) utf8.Length);
            }

            Assert.AreEqual(a, aa.ToString());
            aa.AssertNullTerminated();

            aa.Append("tail");
            Assert.AreEqual(a + "tail", aa.ToString());
            aa.AssertNullTerminated();

            aa.Dispose();
        }

        [TestCase("red")]
        [TestCase("紅色", TestName = "{m}(Chinese-Red)")]
        [TestCase("George Washington")]
        [TestCase("村上春樹", TestName = "{m}(HarukiMurakami)")]
        public void NativeTextToStringWorks(String a)
        {
            NativeText aa = new NativeText(4, Allocator.Temp);
            aa.Append(new FixedString128Bytes(a));
            Assert.AreEqual(a, aa.ToString());
            aa.AssertNullTerminated();
            aa.Dispose();
        }

        [TestCase("monkey", "monkey")]
        [TestCase("yellow", "green")]
        [TestCase("violet", "紅色", TestName = "{m}(Violet-Chinese-Red")]
        [TestCase("绿色", "蓝色", TestName = "{m}(Chinese-Green-Blue")]
        [TestCase("靛蓝色", "紫罗兰色", TestName = "{m}(Chinese-Indigo-Violet")]
        [TestCase("James Monroe", "John Quincy Adams")]
        [TestCase("Andrew Jackson", "村上春樹", TestName = "{m}(AndrewJackson-HarukiMurakami")]
        [TestCase("三島 由紀夫", "吉本ばなな", TestName = "{m}(MishimaYukio-YoshimotoBanana")]
        public void NativeTextEqualsWorks(String a, String b)
        {
            NativeText aa = new NativeText(new FixedString128Bytes(a), Allocator.Temp);
            NativeText bb = new NativeText(new FixedString128Bytes(b), Allocator.Temp);
            Assert.AreEqual(aa.Equals(bb), a.Equals(b));
            aa.AssertNullTerminated();
            bb.AssertNullTerminated();
            aa.Dispose();
            bb.Dispose();
        }

        [Test]
        public void NativeTextForEach()
        {
            NativeText actual = new NativeText("A🌕Z🌑D𒁃 𒁃𒁃𒁃𒁃𒁃", Allocator.Temp);
            FixedList128Bytes<uint> expected = default;
            expected.Add('A');
            expected.Add(0x1F315);
            expected.Add('Z');
            expected.Add(0x1F311);
            expected.Add('D');
            expected.Add(0x12043);
            expected.Add(' ');
            expected.Add(0x12043);
            expected.Add(0x12043);
            expected.Add(0x12043);
            expected.Add(0x12043);
            expected.Add(0x12043);
            int index = 0;
            foreach (var rune in actual)
            {
                Assert.AreEqual(expected[index], rune.value);
                ++index;
            }

            actual.Dispose();
        }

        [Test]
        public void NativeTextIndexOf()
        {
            NativeText a = new NativeText("bookkeeper bookkeeper", Allocator.Temp);
            NativeText b = new NativeText("ookkee", Allocator.Temp);
            Assert.AreEqual(1, a.IndexOf(b));
            Assert.AreEqual(-1, b.IndexOf(a));
            a.Dispose();
            b.Dispose();
        }

        [Test]
        public void NativeTextLastIndexOf()
        {
            NativeText a = new NativeText("bookkeeper bookkeeper", Allocator.Temp);
            NativeText b = new NativeText("ookkee", Allocator.Temp);
            Assert.AreEqual(12, a.LastIndexOf(b));
            Assert.AreEqual(-1, b.LastIndexOf(a));
            a.Dispose();
            b.Dispose();
        }

        [Test]
        public void NativeTextContains()
        {
            NativeText a = new NativeText("bookkeeper", Allocator.Temp);
            NativeText b = new NativeText("ookkee", Allocator.Temp);
            Assert.AreEqual(true, a.Contains(b));
            a.Dispose();
            b.Dispose();
        }

        [Test]
        public void NativeTextComparisons()
        {
            NativeText a = new NativeText("apple", Allocator.Temp);
            NativeText b = new NativeText("banana", Allocator.Temp);
            Assert.AreEqual(false, a.Equals(b));
            Assert.AreEqual(true, !b.Equals(a));
            a.Dispose();
            b.Dispose();
        }

        [Test]
        public void NativeText_CustomAllocatorTest()
        {
            AllocatorManager.Initialize();
            var allocatorHelper = new AllocatorHelper<CustomAllocatorTests.CountingAllocator>(AllocatorManager.Persistent);
            ref var allocator = ref allocatorHelper.Allocator;
            allocator.Initialize();

            using (var container = new NativeText(allocator.Handle))
            {
            }

            Assert.IsTrue(allocator.WasUsed);
            allocator.Dispose();
            allocatorHelper.Dispose();
            AllocatorManager.Shutdown();
        }

        [BurstCompile]
        struct BurstedCustomAllocatorJob : IJob
        {
            [NativeDisableUnsafePtrRestriction]
            public unsafe CustomAllocatorTests.CountingAllocator* Allocator;

            public void Execute()
            {
                unsafe
                {
                    using (var container = new NativeText(Allocator->Handle))
                    {
                    }
                }
            }
        }

        [Test]
        public unsafe void NativeText_BurstedCustomAllocatorTest()
        {
            AllocatorManager.Initialize();
            var allocatorHelper = new AllocatorHelper<CustomAllocatorTests.CountingAllocator>(AllocatorManager.Persistent);
            ref var allocator = ref allocatorHelper.Allocator;
            allocator.Initialize();

            var allocatorPtr = (CustomAllocatorTests.CountingAllocator*)UnsafeUtility.AddressOf<CustomAllocatorTests.CountingAllocator>(ref allocator);
            unsafe
            {
                var handle = new BurstedCustomAllocatorJob {Allocator = allocatorPtr}.Schedule();
                handle.Complete();
            }

            Assert.IsTrue(allocator.WasUsed);
            allocator.Dispose();
            allocatorHelper.Dispose();
            AllocatorManager.Shutdown();
        }
    }
}
#endif
