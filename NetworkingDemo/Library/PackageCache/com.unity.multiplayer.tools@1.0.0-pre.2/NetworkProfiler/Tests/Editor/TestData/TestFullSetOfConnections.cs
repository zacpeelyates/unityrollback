using Unity.Multiplayer.Tools.NetworkProfiler.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    class TestFullSetOfConnections : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Test/FullSetOfConnections", false, 22)]
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
            var window = (TestFullSetOfConnections) GetWindow(typeof(TestFullSetOfConnections));
            window.Show();
        }

        void SetupTreeView()
        {
            var bar = new ColumnBarNetwork();
            var treeView = new TreeViewNetwork(FakeData.FullSetOfConnections);
            treeView.Show(TreeViewNetwork.DisplayType.Messages);
            var holder = new VisualElement {name = "tree view"};
            bar.NameClickEvent += up => Debug.Log($"Clicked Name {up}");
            bar.BytesSentClickEvent += up => Debug.Log($"Clicked Bytes Sent {up}");
            bar.BytesReceivedClickEvent += up => Debug.Log($"Clicked Bytes Received {up}");
            holder.style.flexGrow = 1;
            rootVisualElement.Add(bar);
            holder.Add(treeView);
            rootVisualElement.Add(holder);
        }
    }
}