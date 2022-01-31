using Unity.Multiplayer.Tools.NetworkProfiler.Editor;
using UnityEditor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    class TestSearchPartialFilter : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Test/Test Search Partial Filter", false, 41)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (TestSearchPartialFilter) GetWindow(typeof(TestSearchPartialFilter));
            window.Show();
        }

        void OnEnable()
        {
            SetupSearchBar();
        }

        void SetupSearchBar()
        {
            var columnBarNetwork = new ColumnBarNetwork();
            var listViewContainer = new ListViewContainer();
            var searchBar = new SearchBar();
            searchBar.SetEntries(FakeData.FullSetOfConnections);

            searchBar.OnSearchResultsChanged += list =>
            {
                listViewContainer.CacheResults(list);
                listViewContainer.ShowResults();
                rootVisualElement.Add(listViewContainer);
            };
            searchBar.OnSearchStringCleared += () =>
            {
                listViewContainer.ClearResults();
            };

            rootVisualElement.Add(searchBar);
            rootVisualElement.Add(columnBarNetwork);
        }
    }
}