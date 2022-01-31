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
    /// You can pass three integral or floating point parameters alongside the Begin event.
    /// The following types are supported as parameters:
    /// * int
    /// * uint
    /// * long
    /// * ulong
    /// * float
    /// * double
    /// </summary>
    /// <typeparam name="TP1">Type of the first parameter.</typeparam>
    /// <typeparam name="TP2">Type of the second parameter.</typeparam>
    /// <typeparam name="TP3">Type of the third parameter.</typeparam>
#if ENABLE_PROFILER
    [StructLayout(LayoutKind.Sequential)]
#else
    [StructLayout(LayoutKind.Sequential, Size = 0)]
#endif
    public readonly struct ProfilerMarker<TP1, TP2, TP3>
        where TP1 : unmanaged
        where TP2 : unmanaged
        where TP3 : unmanaged
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
        [NonSerialized]
        readonly byte m_P2Type;
        [NonSerialized]
        readonly byte m_P3Type;
#endif

        /// <summary>
        /// Constructs a ProfilerMarker that belongs to the generic ProfilerCategory.Scripts category.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="name">Name of a marker.</param>
        /// <param name="param1Name">Name of the first parameter.</param>
        /// <param name="param2Name">Name of the second parameter.</param>
        /// <param name="param3Name">Name of the third parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerMarker(string name, string param1Name, string param2Name, string param3Name)
        {
#if ENABLE_PROFILER
            m_P1Type = ProfilerUtility.GetProfilerMarkerDataType<TP1>();
            m_P2Type = ProfilerUtility.GetProfilerMarkerDataType<TP2>();
            m_P3Type = ProfilerUtility.GetProfilerMarkerDataType<TP3>();
            m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, ProfilerUnsafeUtility.CategoryScripts, MarkerFlags.Default, 3);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 0, param1Name, m_P1Type, (byte)ProfilerMarkerDataUnit.Undefined);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 1, param2Name, m_P2Type, (byte)ProfilerMarkerDataUnit.Undefined);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 2, param3Name, m_P3Type, (byte)ProfilerMarkerDataUnit.Undefined);
#endif
        }

        /// <summary>
        /// Constructs a ProfilerMarker.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="category">Profiler category.</param>
        /// <param name="name">Name of a marker.</param>
        /// <param name="param1Name">Name of the first parameter.</param>
        /// <param name="param2Name">Name of the second parameter.</param>
        /// <param name="param3Name">Name of the third parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerMarker(ProfilerCategory category, string name, string param1Name, string param2Name, string param3Name)
        {
#if ENABLE_PROFILER
            m_P1Type = ProfilerUtility.GetProfilerMarkerDataType<TP1>();
            m_P2Type = ProfilerUtility.GetProfilerMarkerDataType<TP2>();
            m_P3Type = ProfilerUtility.GetProfilerMarkerDataType<TP3>();
            m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, category, MarkerFlags.Default, 3);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 0, param1Name, m_P1Type, (byte)ProfilerMarkerDataUnit.Undefined);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 1, param2Name, m_P2Type, (byte)ProfilerMarkerDataUnit.Undefined);
            ProfilerUnsafeUtility.SetMarkerMetadata(m_Ptr, 2, param3Name, m_P3Type, (byte)ProfilerMarkerDataUnit.Undefined);
#endif
        }

        /// <summary>
        /// Begin profiling a piece of code marked with the ProfilerMarker instance.
        /// </summary>
        /// <remarks>Does nothing in Release Players.</remarks>
        /// <param name="p1">The first context parameter.</param>
        /// <param name="p2">The second context parameter.</param>
        /// <param name="p3">The third context parameter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_PROFILER")]
        [Pure]
        public unsafe void Begin(TP1 p1, TP2 p2, TP3 p3)
        {
#if ENABLE_PROFILER
            var data = stackalloc ProfilerMarkerData[3];
            data[0].Type = m_P1Type;
            data[0].Size = (uint)UnsafeUtility.SizeOf<TP1>();
            data[0].Ptr = UnsafeUtility.AddressOf(ref p1);
            data[1].Type = m_P2Type;
            data[1].Size = (uint)UnsafeUtility.SizeOf<TP2>();
            data[1].Ptr = UnsafeUtility.AddressOf(ref p2);
            data[2].Type = m_P3Type;
            data[2].Size = (uint)UnsafeUtility.SizeOf<TP3>();
            data[2].Ptr = UnsafeUtility.AddressOf(ref p3);
            ProfilerUnsafeUtility.BeginSampleWithMetadata(m_Ptr, 3, data);
#endif
        }

        /// <summary>
        /// End profiling a piece of code marked with the ProfilerMarker instance.
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
        /// A helper struct that automatically calls End on Dispose and is used with ''using'' statement.
        /// </summary>
        public readonly struct AutoScope : IDisposable
        {
#if ENABLE_PROFILER
            readonly ProfilerMarker<TP1, TP2, TP3> m_Marker;
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal AutoScope(ProfilerMarker<TP1, TP2, TP3> marker, TP1 p1, TP2 p2, TP3 p3)
            {
#if ENABLE_PROFILER
                m_Marker = marker;
                m_Marker.Begin(p1, p2, p3);
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
        /// Use to profile a piece of code enclosed within *using* statement.
        /// </summary>
        /// <remarks>Returns *null* in Release Players.</remarks>
        /// <param name="p1">The first context parameter.</param>
        /// <param name="p2">The second context parameter.</param>
        /// <param name="p3">The third context parameter.</param>
        /// <returns>IDisposable struct which calls End on Dispose.</returns>
        /// <example>
        /// <code>
        /// using (profilerMarker.Auto(enemies.Count, blastRadius, blastPos.x))
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
        public AutoScope Auto(TP1 p1, TP2 p2, TP3 p3)
        {
#if ENABLE_PROFILER
            return new AutoScope(this, p1, p2, p3);
#else
            return default;
#endif
        }
    }
}
