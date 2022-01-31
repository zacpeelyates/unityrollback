using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.Tests;

#if !UNITY_DOTSRUNTIME && ENABLE_UNITY_COLLECTIONS_CHECKS
internal class RewindableAllocatorTests : CollectionsTestFixture
{
    [Test]
    public unsafe void RewindTestVersionOverflow()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);

        // Check allocator version overflow
        for (int i = 0; i < 65536 + 100; i++)
        {
            var container = allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes / 1000);
            container.Resize(1, NativeArrayOptions.ClearMemory);
            container[0] = 0xFE;
            allocator.Rewind();
            CollectionHelper.CheckAllocator(allocator.ToAllocator);
        }
        allocator.Dispose();
        allocatorHelper.Dispose();
    }


    public unsafe void RewindInvalidatesNativeList()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        var container = allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes / 1000);
        container.Resize(1, NativeArrayOptions.ClearMemory);
        container[0] = 0xFE;
        allocator.Rewind();
        Assert.Throws<ObjectDisposedException>(() =>
        {
            container[0] = 0xEF;
        });
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public unsafe void RewindInvalidatesNativeArray()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        var container = allocator.AllocateNativeArray<byte>(allocator.InitialSizeInBytes / 1000);
        container[0] = 0xFE;
        allocator.Rewind();
        Assert.Throws<ObjectDisposedException>(() =>
        {
            container[0] = 0xEF;
        });
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public unsafe void NativeListCanBeCreatedViaMemberFunction()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        var container = allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes / 1000);
        container.Resize(1, NativeArrayOptions.ClearMemory);
        container[0] = 0xFE;
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public unsafe void NativeListCanBeDisposed()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        var container = allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes / 1000);
        container.Resize(1, NativeArrayOptions.ClearMemory);
        container[0] = 0xFE;
        container.Dispose();
        allocator.Rewind();
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public void NativeArrayCanBeDisposed()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        var container = allocator.AllocateNativeArray<byte>(allocator.InitialSizeInBytes / 1000);
        container[0] = 0xFE;
        container.Dispose();
        allocator.Rewind();
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public void NumberOfBlocksIsTemporarilyStable()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes * 10);
        var blocksBefore = allocator.BlocksAllocated;
        allocator.Rewind();
        var blocksAfter = allocator.BlocksAllocated;
        Assert.AreEqual(blocksAfter, blocksBefore);
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public void NumberOfBlocksEventuallyDrops()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes * 10);
        var blocksBefore = allocator.BlocksAllocated;
        allocator.Rewind();
        allocator.Rewind();
        var blocksAfter = allocator.BlocksAllocated;
        Assert.IsTrue(blocksAfter < blocksBefore);
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public void PossibleToAllocateGigabytes()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        const int giga = 1024 * 1024 * 1024;
        var container0 = allocator.AllocateNativeList<byte>(giga);
        var container1 = allocator.AllocateNativeList<byte>(giga);
        var container2 = allocator.AllocateNativeList<byte>(giga);
        container0.Resize(1, NativeArrayOptions.ClearMemory);
        container1.Resize(1, NativeArrayOptions.ClearMemory);
        container2.Resize(1, NativeArrayOptions.ClearMemory);
        container0[0] = 0;
        container1[0] = 1;
        container2[0] = 2;
        Assert.AreEqual((byte)0, container0[0]);
        Assert.AreEqual((byte)1, container1[0]);
        Assert.AreEqual((byte)2, container2[0]);
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    [Test]
    public void ExhaustsFirstBlockBeforeAllocatingMore()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);
        for (var i = 0; i < 50; ++i)
        {
            allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes / 100);
            Assert.AreEqual(1, allocator.BlocksAllocated);
        }
        allocator.AllocateNativeList<byte>(allocator.InitialSizeInBytes);
        Assert.AreEqual(2, allocator.BlocksAllocated);
        allocator.Dispose();
        allocatorHelper.Dispose();
    }

    unsafe struct ListProvider
    {
        NativeList<byte> m_Bytes;

        public ListProvider(AllocatorManager.AllocatorHandle allocatorHandle) => m_Bytes = new NativeList<byte>(allocatorHandle);

        public void Append<T>(ref T data) where T : unmanaged =>
            m_Bytes.AddRange(UnsafeUtility.AddressOf(ref data), UnsafeUtility.SizeOf<T>());
    }

    static void TriggerBug(AllocatorManager.AllocatorHandle allocatorHandle, NativeArray<byte> data)
    {
        var listProvider = new ListProvider(allocatorHandle);

        var datum = 0u;
        listProvider.Append(ref datum); // 'data' is now invalid after call to AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);

        Assert.That(data[0], Is.EqualTo(0));
    }

    [Test]
    public void AddRange_WhenCalledOnStructMember_DoesNotInvalidateUnrelatedListHigherOnCallStack()
    {
        var allocatorHelper = new AllocatorHelper<RewindableAllocator>(AllocatorManager.Persistent);
        ref var allocator = ref allocatorHelper.Allocator;
        allocator.Initialize(1024 * 1024);

        AllocatorManager.AllocatorHandle allocatorHandle = allocator.Handle;

        var unrelatedList = new NativeList<byte>(allocatorHandle) { 0, 0 };
        Assert.That(unrelatedList.Length, Is.EqualTo(2));
        Assert.That(unrelatedList[0], Is.EqualTo(0));

        TriggerBug(allocatorHandle, unrelatedList);

        allocator.Dispose();
        allocatorHandle.Dispose();
    }
}

#endif
