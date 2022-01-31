using Unity.Multiplayer.Tools.NetworkProfiler.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    class TestSingleFullConnections : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Test/SingleFullConnections", false, 22)]
        public static void Init()
        {
            ShowWindow();
        }

        void OnEnable()
        {
            SetupTreeView();
        }

        static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            var window = (TestSingleFullConnections) GetWindow(typeof(TestSingleFullConnections));
            window.Show();
        }

        void SetupTreeView()
        {
            var bar = new ColumnBarNetwork();
            var treeView = new TreeViewNetwork(FakeData.SingleFullConnections);
            treeView.Show(TreeViewNetwork.DisplayType.Activity);
            var holder = new VisualElement {name = "tree view"};
            holder.style.flexGrow = 1;
            rootVisualElement.Add(bar);
            holder.Add(treeView);
            rootVisualElement.Add(holder);
        }
    }
}