using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;

namespace Unity.Profiling
{
    /// <summary>
    /// Provides an extension to the ProfilerMarker API to accommodate a single additional parameter to the Begin method.
    /// </summary>
    public static class ProfilerMarkerExtension
    {
        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="marker">ProfilerMarker instance.</param>
        /// <param name="metadata">''int'' parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public static unsafe void Begin(this ProfilerMarker marker, int metadata)
        {
            var data = new ProfilerMarkerData
            {
                Type = (byte)ProfilerMarkerDataType.Int32,
                Size = (uint)UnsafeUtility.SizeOf<int>(),
                Ptr = &metadata
            };
            ProfilerUnsafeUtility.BeginSampleWithMetadata(marker.Handle, 1, &data);
        }

        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="marker">ProfilerMarker instance.</param>
        /// <param name="metadata">''uint'' parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public static unsafe void Begin(this ProfilerMarker marker, uint metadata)
        {
            var data = new ProfilerMarkerData
            {
                Type = (byte)ProfilerMarkerDataType.UInt32,
                Size = (uint)UnsafeUtility.SizeOf<uint>(),
                Ptr = &metadata
            };
            ProfilerUnsafeUtility.BeginSampleWithMetadata(marker.Handle, 1, &data);
        }

        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="marker">ProfilerMarker instance.</param>
        /// <param name="metadata">''long'' parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public static unsafe void Begin(this ProfilerMarker marker, long metadata)
        {
            var data = new ProfilerMarkerData
            {
                Type = (byte)ProfilerMarkerDataType.Int64,
                Size = (uint)UnsafeUtility.SizeOf<long>(),
                Ptr = &metadata
            };
            ProfilerUnsafeUtility.BeginSampleWithMetadata(marker.Handle, 1, &data);
        }

        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="marker">ProfilerMarker instance.</param>
        /// <param name="metadata">''ulong'' parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public static unsafe void Begin(this ProfilerMarker marker, ulong metadata)
        {
            var data = new ProfilerMarkerData
            {
                Type = (byte)ProfilerMarkerDataType.UInt64,
                Size = (uint)UnsafeUtility.SizeOf<ulong>(),
                Ptr = &metadata
            };
            ProfilerUnsafeUtility.BeginSampleWithMetadata(marker.Handle, 1, &data);
        }

        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="marker">ProfilerMarker instance.</param>
        /// <param name="metadata">''float'' parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public static unsafe void Begin(this ProfilerMarker marker, float metadata)
        {
            var data = new ProfilerMarkerData
            {
                Type = (byte)ProfilerMarkerDataType.Float,
                Size = (uint)UnsafeUtility.SizeOf<float>(),
                Ptr = &metadata
            };
            ProfilerUnsafeUtility.BeginSampleWithMetadata(marker.Handle, 1, &data);
        }

        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="marker">ProfilerMarker instance.</param>
        /// <param name="metadata">''double'' parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public static unsafe void Begin(this ProfilerMarker marker, double metadata)
        {
            var data = new ProfilerMarkerData
            {
                Type = (byte)ProfilerMarkerDataType.Double,
                Size = (uint)UnsafeUtility.SizeOf<double>(),
                Ptr = &metadata
            };
            ProfilerUnsafeUtility.BeginSampleWithMetadata(marker.Handle, 1, &data);
        }

        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="marker">ProfilerMarker instance.</param>
        /// <param name="metadata">''string'' parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public static unsafe void Begin(this ProfilerMarker marker, string metadata)
        {
            var data = new ProfilerMarkerData { Type = (byte)ProfilerMarkerDataType.String16 };
            fixed(char* c = metadata)
            {
                data.Size = ((uint)metadata.Length + 1) * 2;
                data.Ptr = c;
                ProfilerUnsafeUtility.BeginSampleWithMetadata(marker.Handle, 1, &data);
            }
        }
    }
}
