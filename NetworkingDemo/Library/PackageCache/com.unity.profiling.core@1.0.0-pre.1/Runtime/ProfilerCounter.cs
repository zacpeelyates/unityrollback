using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;

namespace Unity.Profiling
{
    /// <summary>
    /// Reports a value of an integer or floating point type to the Unity Profiler.
    /// </summary>
    /// <typeparam name="T">int, uint, long, ulong, float or double type.</typeparam>
#if ENABLE_PROFILER
    [StructLayout(LayoutKind.Sequential)]
#else
    [StructLayout(LayoutKind.Sequential, Size = 0)]
#endif
    public readonly struct ProfilerCounter<T> where T : unmanaged
    {
#if ENABLE_PROFILER
        [NativeDisableUnsafePtrRestriction]
        [NonSerialized]
        readonly IntPtr m_Ptr;

        [NonSerialized]
        readonly byte m_Type;
#endif


        /// <summary>
        /// Constructs a **ProfilerCounter** that is reported to the Unity Profiler whenever you call Sample().
        /// </summary>
        /// <param name="category">Profiler category.</param>
        /// <param name="name">Name of ProfilerCounter.</param>
        /// <param name="dataUnit">Value unit type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerCounter(ProfilerCategory category, string name, ProfilerMarkerDataUnit dataUnit)
        {
#if ENABLE_PROFILER
            m_Type = ProfilerUtility.GetProfilerMarkerDataType<T>();
            m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, category, MarkerFlags.Counter, 1);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 0, null, m_Type, (byte)dataUnit);
#endif
        }

        /// <summary>
        /// Sends the value to Unity Profiler immediately.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="value">The value to send to the profiler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public void Sample(T value)
        {
#if ENABLE_PROFILER
            unsafe
            {
                var data = new ProfilerMarkerData
                {
                    Type = m_Type,
                    Size = (uint)UnsafeUtility.SizeOf<T>(),
                    Ptr = UnsafeUtility.AddressOf(ref value)
                };
                ProfilerUnsafeUtility.SingleSampleWithMetadata(m_Ptr, 1, &data);
            }
#endif
        }
    }
}
