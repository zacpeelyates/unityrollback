#if !UNITY_2020_2_OR_NEWER

using Unity.Profiling.LowLevel.Unsafe;

namespace Unity.Profiling
{
    /// <summary>
    /// Defines a profiling category when you create a ProfilerMarker.
    /// </summary>
    /// <seealso cref="ProfilerMarker{TP1}"/>
    public struct ProfilerCategory
    {
        readonly ushort m_Category;

        ProfilerCategory(ushort category)
        {
            m_Category = category;
        }

        /// <summary>
        /// A ProfilerMarker that belongs to the Render system.
        /// </summary>
        public static ProfilerCategory Render => new ProfilerCategory(ProfilerUnsafeUtility.CategoryRender);
        /// <summary>
        /// Default category for all ProfilerMarkers defined in scripting code.
        /// </summary>
        public static ProfilerCategory Scripts => new ProfilerCategory(ProfilerUnsafeUtility.CategoryScripts);
        /// <summary>
        /// A ProfilerMarker that belongs to the UI system.
        /// </summary>
        public static ProfilerCategory GUI => new ProfilerCategory(ProfilerUnsafeUtility.CategoryGUI);
        /// <summary>
        /// A ProfilerMarker that belongs to the Physics system.
        /// </summary>
        public static ProfilerCategory Physics => new ProfilerCategory(ProfilerUnsafeUtility.CategoryPhysics);
        /// <summary>
        /// A ProfilerMarker that belongs to the Animation system.
        /// </summary>
        public static ProfilerCategory Animation => new ProfilerCategory(ProfilerUnsafeUtility.CategoryAnimation);
        /// <summary>
        /// A ProfilerMarker that belongs to the Ai or NavMesh system.
        /// </summary>
        public static ProfilerCategory Ai => new ProfilerCategory(ProfilerUnsafeUtility.CategoryAi);
        /// <summary>
        /// A ProfilerMarker that belongs the to Audio system.
        /// </summary>
        public static ProfilerCategory Audio => new ProfilerCategory(ProfilerUnsafeUtility.CategoryAudio);
        /// <summary>
        /// A ProfilerMarker that belongs to the Video system.
        /// </summary>
        public static ProfilerCategory Video => new ProfilerCategory(ProfilerUnsafeUtility.CategoryVideo);
        /// <summary>
        /// A ProfilerMarker that belongs to the Particle system.
        /// </summary>
        public static ProfilerCategory Particles => new ProfilerCategory(ProfilerUnsafeUtility.CategoryParticles);
        /// <summary>
        /// A ProfilerMarker that belongs to the Lighting system.
        /// </summary>
        public static ProfilerCategory Lighting => new ProfilerCategory(ProfilerUnsafeUtility.CategoryLightning);
        /// <summary>
        /// A ProfilerMarker that belongs to the Networking system.
        /// </summary>
        public static ProfilerCategory Network => new ProfilerCategory(ProfilerUnsafeUtility.CategoryNetwork);
        /// <summary>
        /// A ProfilerMarker that belongs to the Loading or Streaming system.
        /// </summary>
        public static ProfilerCategory Loading => new ProfilerCategory(ProfilerUnsafeUtility.CategoryLoading);
        /// <summary>
        /// A ProfilerMarker that belongs to the VR system.
        /// </summary>
        public static ProfilerCategory Vr => new ProfilerCategory(ProfilerUnsafeUtility.CategoryVr);
        /// <summary>
        /// A ProfilerMarker that belongs to the Input system.
        /// </summary>
        public static ProfilerCategory Input => new ProfilerCategory(ProfilerUnsafeUtility.CategoryInput);

        /// <summary>
        /// Utility operator that simplifies usage of the ProfilerCategory with ProfilerUnsafeUtility.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>ProfilerCategory value as UInt16.</returns>
        public static implicit operator ushort(ProfilerCategory category)
        {
            return category.m_Category;
        }
    }
}

#endif
