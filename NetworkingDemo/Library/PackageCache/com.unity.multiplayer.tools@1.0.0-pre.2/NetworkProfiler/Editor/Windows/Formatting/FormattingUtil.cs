using UnityEditor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class FormattingUtil
    {
        public static string FormatBytesForDetailsView(long bytes) =>
            bytes == 0 ? "-" : EditorUtility.FormatBytes(bytes);
    }
}