using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;

namespace Unity.Profiling
{
    /// <summary>
    /// Reports a value of an integral or floating point type to the Unity Profiler.
    /// </summary>
    /// <typeparam name="T">int, uint, long, ulong, float or double type.</typeparam>
#if ENABLE_PROFILER
    [StructLayout(LayoutKind.Sequential)]
#else
    [StructLayout(LayoutKind.Sequential, Size = 0)]
#endif
    public readonly struct ProfilerCounterValue<T> where T : unmanaged
    {
#if ENABLE_PROFILER
        [NativeDisableUnsafePtrRestriction]
        [NonSerialized]
        readonly unsafe T* m_Value;
#endif

        /// <summary>
        /// Constructs a **ProfilerCounter** that belongs to the generic ProfilerCategory.Scripts category. It is reported at the end of CPU frame to the Unity Profiler.
        /// </summary>
        /// <param name="name">Name of ProfilerCounter.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerCounterValue(string name)
        {
#if ENABLE_PROFILER
            byte dataType = ProfilerUtility.GetProfilerMarkerDataType<T>();
            unsafe
            {
                m_Value = (T*)ProfilerUnsafeUtility.CreateCounterValue(out var counterPtr, name, ProfilerUnsafeUtility.CategoryScripts, MarkerFlags.Default, dataType, (byte)ProfilerMarkerDataUnit.Undefined, UnsafeUtility.SizeOf<T>(), ProfilerCounterOptions.FlushOnEndOfFrame);
            }
#endif
        }

        /// <summary>
        /// Constructs a **ProfilerCounter** that belongs to the generic ProfilerCategory.Scripts category. It is reported at the end of CPU frame to the Unity Profiler.
        /// </summary>
        /// <param name="name">Name of ProfilerCounter.</param>
        /// <param name="dataUnit">Value unit type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerCounterValue(string name, ProfilerMarkerDataUnit dataUnit)
        {
#if ENABLE_PROFILER
            byte dataType = ProfilerUtility.GetProfilerMarkerDataType<T>();
            unsafe
            {
                m_Value = (T*)ProfilerUnsafeUtility.CreateCounterValue(out var counterPtr, name, ProfilerUnsafeUtility.CategoryScripts, MarkerFlags.Default, dataType, (byte)dataUnit, UnsafeUtility.SizeOf<T>(), ProfilerCounterOptions.FlushOnEndOfFrame);
            }
#endif
        }

        /// <summary>
        /// Constructs a **ProfilerCounter** that belongs to generic ProfilerCategory.Scripts category.
        /// </summary>
        /// <param name="name">Name of ProfilerCounter.</param>
        /// <param name="dataUnit">Value unit type.</param>
        /// <param name="counterOptions">ProfilerCounter options.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerCounterValue(string name, ProfilerMarkerDataUnit dataUnit, ProfilerCounterOptions counterOptions)
        {
#if ENABLE_PROFILER
            byte dataType = ProfilerUtility.GetProfilerMarkerDataType<T>();
            unsafe
            {
                m_Value = (T*)ProfilerUnsafeUtility.CreateCounterValue(out var counterPtr, name, ProfilerUnsafeUtility.CategoryScripts, MarkerFlags.Default, dataType, (byte)dataUnit, UnsafeUtility.SizeOf<T>(), counterOptions);
            }
#endif
        }

        /// <summary>
        /// Constructs a **ProfilerCounter** that is reported at the end of CPU frame to the Unity Profiler.
        /// </summary>
        /// <param name="category">Profiler category.</param>
        /// <param name="name">Name of ProfilerCounter.</param>
        /// <param name="dataUnit">Value unit type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerCounterValue(ProfilerCategory category, string name, ProfilerMarkerDataUnit dataUnit)
        {
#if ENABLE_PROFILER
            byte dataType = ProfilerUtility.GetProfilerMarkerDataType<T>();
            unsafe
            {
                m_Value = (T*)ProfilerUnsafeUtility.CreateCounterValue(out var counterPtr, name, category, MarkerFlags.Default, dataType, (byte)dataUnit, UnsafeUtility.SizeOf<T>(), ProfilerCounterOptions.FlushOnEndOfFrame);
            }
#endif
        }

        /// <summary>
        /// Constructs a **ProfilerCounter**.
        /// </summary>
        /// <param name="category">Profiler category.</param>
        /// <param name="name">Name of ProfilerCounter.</param>
        /// <param name="dataUnit">Value unit type.</param>
        /// <param name="counterOptions">ProfilerCounter options.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProfilerCounterValue(ProfilerCategory category, string name, ProfilerMarkerDataUnit dataUnit, ProfilerCounterOptions counterOptions)
        {
#if ENABLE_PROFILER
            byte dataType = ProfilerUtility.GetProfilerMarkerDataType<T>();
            unsafe
            {
                m_Value = (T*)ProfilerUnsafeUtility.CreateCounterValue(out var counterPtr, name, category, MarkerFlags.Default, dataType, (byte)dataUnit, UnsafeUtility.SizeOf<T>(), counterOptions);
            }
#endif
        }

        /// <summary>
        /// Gets or sets value of the ProfilerCounter.
        /// </summary>
        /// <remarks>Returns 0 and is not implemented in Release Players.</remarks>
        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if ENABLE_PROFILER
                unsafe
                {
                    return *m_Value;
                }
#else
                return default;
#endif
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
#if ENABLE_PROFILER
                unsafe
                {
                    *m_Value = value;
                }
#endif
            }
        }

        /// <summary>
        /// Sends the value to Unity Profiler immediately.
        /// </summary>
        [Conditional("ENABLE_PROFILER")]
        public void Sample()
        {
#if ENABLE_PROFILER
            unsafe
            {
                ProfilerUnsafeUtility.FlushCounterValue(m_Value);
            }
#endif
        }
    }
}
