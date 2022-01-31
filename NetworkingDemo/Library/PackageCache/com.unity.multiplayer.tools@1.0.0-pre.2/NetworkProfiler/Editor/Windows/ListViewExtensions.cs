using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class ListViewExtensions
    {

        /// This is a version-safe method of getting the item height, motivated by the following conundrum:
        /// 1. `int itemHeight {get; set; }` and `float resolvedItemHeight { get; }` are deprecated as of 2021.2
        /// 2. `float fixedItemHeight {get; set}` was introduced in 2021.2
        /// It's a method and not a property because although extension properties were proposed for C# 8
        /// they didn't make it in.
        public static float GetItemHeight(this ListView listView)
        {
#if UNITY_2021_2_OR_NEWER
            // `float fixedItemHeight` was added in 2021.2
            return listView.fixedItemHeight;
#else
            // `int itemHeight` is deprecated as of 2021.2 (as is `float resolvedItemHeight`)
            return listView.itemHeight;
#endif
        }

        /// This is a version-safe method of setting the item height, motivated by the following conundrum:
        /// 1. `int itemHeight {get; set; }` and `float resolvedItemHeight { get; }` are deprecated as of 2021.2
        /// 2. `float fixedItemHeight {get; set}` was introduced in 2021.2
        /// It's a method and not a property because although extension properties were proposed for C# 8
        /// they didn't make it in.
        public static void SetItemHeight(this ListView listView, float value)
        {
#if UNITY_2021_2_OR_NEWER
            // `float fixedItemHeight` was added in 2021.2
            listView.fixedItemHeight = value;
#else
            // `int itemHeight` is deprecated as of 2021.2 (as is `float resolvedItemHeight`)
            listView.itemHeight = (int)value;
#endif
        }

        /// This is a version-safe method of setting the item height, motivated by the following conundrum:
        /// 1. `int itemHeight {get; set; }` and `float resolvedItemHeight { get; }` are deprecated as of 2021.2
        /// 2. `float fixedItemHeight {get; set}` was introduced in 2021.2
        /// It's a method and not a property because although extension properties were proposed for C# 8
        /// they didn't make it in.
        public static void SetItemHeight(this ListView listView, int value)
        {
#if UNITY_2021_2_OR_NEWER
            // `float fixedItemHeight` was added in 2021.2
            listView.fixedItemHeight = value;
#else
            // `int itemHeight` is deprecated as of 2021.2 (as is `float resolvedItemHeight`)
            listView.itemHeight = value;
#endif
        }

#if !UNITY_2021_2_OR_NEWER
        public static void Rebuild(this ListView listView)
        {
            // Not implemented; no-op extension to allow compilation in older versions
            // without broad-form ifdefs
        }
#endif
    }
}
