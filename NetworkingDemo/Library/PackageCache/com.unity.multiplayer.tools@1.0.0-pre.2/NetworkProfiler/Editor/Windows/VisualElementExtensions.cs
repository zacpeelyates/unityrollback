using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal static class VisualElementExtensions
    {
        public static void SetRotate(this VisualElement element, float rotation)
        {
#if UNITY_2021_2_OR_NEWER
            element.style.rotate = new StyleRotate(new Rotate(rotation));
#endif
        }
    }
}
