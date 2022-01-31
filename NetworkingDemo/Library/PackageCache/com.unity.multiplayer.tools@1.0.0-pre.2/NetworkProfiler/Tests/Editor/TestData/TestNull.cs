using Unity.Multiplayer.Tools.NetworkProfiler.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    class TestNull : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Test/Null", false, 21)]
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
            var window = (TestNull) GetWindow(typeof(TestNull));
            window.Show();
        }

        void SetupTreeView()
        {
            var bar = new ColumnBarNetwork();
            var treeView = new TreeViewNetwork(null);
            treeView.Show(TreeViewNetwork.DisplayType.Activity);
            var holder = new VisualElement {name = "tree view"};
            holder.style.flexGrow = 1;
            rootVisualElement.Add(bar);
            holder.Add(treeView);
            rootVisualElement.Add(holder);
        }
    }
}