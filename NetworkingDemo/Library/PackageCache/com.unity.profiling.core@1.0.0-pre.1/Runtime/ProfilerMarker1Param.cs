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
    /// Use ProfilerMarker&lt;TP1&gt; to mark up script code blocks for the Unity Profiler.
    ///
    /// You can pass a single integral or floating point parameter alongside the Begin event.
    /// </summary>
    /// <typeparam name="TP1">int, uint, long, ulong, float or double type.</typeparam>
#if ENABLE_PROFILER
    [StructLayout(LayoutKind.Sequential)]
#else
    [StructLayout(LayoutKind.Sequential, Size = 0)]
#endif
    public readonly struct ProfilerMarker<TP1> where TP1 : unmanaged
    {
#if ENABLE_PROFILER
        [NativeDisableUnsafePtrRestriction]
        [NonSerialized]
        readonly IntPtr m_Ptr;
        // m_P1Type is initialized as a member variable to support usage in Burst.
        // Avoiding cctor generation allows us to:
        // 1) Use generic approach.
        // 2) Skip class static init check in Burst code.
        [NonSerialized]
        readonly byte m_P1Type;
#endif

        /// <summary>
        /// Constructs the ProfilerMarker that belongs to the generic ProfilerCategory.Scripts category.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="name">Name of a marker.</param>
        /// <param name="param1Name">Name of the first parameter passed to the Begin method.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerMarker(string name, string param1Name)
        {
#if ENABLE_PROFILER
            m_P1Type = ProfilerUtility.GetProfilerMarkerDataType<TP1>();
            m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, ProfilerUnsafeUtility.CategoryScripts, MarkerFlags.Default, 1);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 0, param1Name, m_P1Type, (byte)ProfilerMarkerDataUnit.Undefined);
#endif
        }

        /// <summary>
        /// Constructs the ProfilerMarker.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="category">Profiler category.</param>
        /// <param name="name">Name of a marker.</param>
        /// <param name="param1Name">Name of the first parameter passed to the Begin method.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerMarker(ProfilerCategory category, string name, string param1Name)
        {
#if ENABLE_PROFILER
            m_P1Type = ProfilerUtility.GetProfilerMarkerDataType<TP1>();
            m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, category, MarkerFlags.Default, 1);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 0, param1Name, m_P1Type, (byte)ProfilerMarkerDataUnit.Undefined);
#endif
        }

        /// <summary>
        /// Begins profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="p1">Additional context parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public unsafe void Begin(TP1 p1)
        {
#if ENABLE_PROFILER
            var data = new ProfilerMarkerData
            {
                Type = m_P1Type,
                Size = (uint)UnsafeUtility.SizeOf<TP1>(),
                Ptr = UnsafeUtility.AddressOf(ref p1)
            };
            ProfilerUnsafeUtility.BeginSampleWithMetadata(m_Ptr, 1, &data);
#endif
        }

        /// <summary>
        /// Ends profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public void End()
        {
#if ENABLE_PROFILER
            ProfilerUnsafeUtility.EndSample(m_Ptr);
#endif
        }

        /// <summary>
        /// A helper struct that automatically calls End on Dispose. Used with the *using* statement.
        /// </summary>
        public readonly struct AutoScope : IDisposable
        {
#if ENABLE_PROFILER
            readonly ProfilerMarker<TP1> m_Marker;
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal AutoScope(ProfilerMarker<TP1> marker, TP1 p1)
            {
#if ENABLE_PROFILER
                m_Marker = marker;
                m_Marker.Begin(p1);
#endif
            }

            /// <summary>
            /// Calls ProfilerMarker.End.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
#if ENABLE_PROFILER
                m_Marker.End();
#endif
            }
        }

        /// <summary>
        /// Profiles a piece of code enclosed within the *using* statement.
        /// </summary>
        /// <remarks>Returns *null* in Release Players.</remarks>
        /// <param name="p1">Additional context parameter.</param>
        /// <returns>IDisposable struct which calls End on Dispose.</returns>
        /// <example>
        /// <code>
        /// using (profilerMarker.Auto(enemies.Count))
        /// {
        ///     var blastRadius2 = blastRadius * blastRadius;
        ///     for (int i = 0; i &lt; enemies.Count; ++i)
        ///     {
        ///         var r2 = (enemies[i].Pos - blastPos).sqrMagnitude;
        ///         if (r2 &lt; blastRadius2)
        ///             enemies[i].Dispose();
        ///     }
        /// }
        /// </code>
        /// </example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public AutoScope Auto(TP1 p1)
        {
#if ENABLE_PROFILER
            return new AutoScope(this, p1);
#else
            return default;
#endif
        }
    }
}
