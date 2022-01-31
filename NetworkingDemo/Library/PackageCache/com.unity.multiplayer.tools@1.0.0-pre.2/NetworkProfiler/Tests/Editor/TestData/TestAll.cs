using UnityEditor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    static class TestAll
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Test/All", false, 1)]
        static void Init()
        {
            TestNull.Init();
            TestSingleFullConnections.Init();
            TestFullSetOfConnections.Init();
        }
    }
}