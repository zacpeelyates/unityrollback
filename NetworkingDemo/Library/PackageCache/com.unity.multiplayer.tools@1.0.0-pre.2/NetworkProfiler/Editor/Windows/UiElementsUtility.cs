using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class UiElementsUtility
    {
        public static string emptyLabelUssClassName
        {
            get
            {
#if UNITY_2021_2_OR_NEWER
                return ListView.emptyLabelUssClassName;
#else
                // Not implemented because it's not currently needed in lower versions
                return string.Empty; 
#endif
            }
        }
    }
}
