using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class BuildTreeViewTabElements
    {
        struct ElementInfo
        {
            public string Name;
            public TreeViewNetwork.DisplayType DisplayType;
        }

        readonly List<ElementInfo> m_ElementInfo = new List<ElementInfo>();

        public static BuildTreeViewTabElements CreateTabElements()
        {
            return new BuildTreeViewTabElements();
        }

        public List<TabInfo> Build()
        {
            var result = new List<TabInfo>();
            foreach (var elementInfo in m_ElementInfo)
            {
                var element = new TreeViewTabElement();
                element.SetTreeViewDisplayType(elementInfo.DisplayType);
                var tabInfo = new TabInfo {Name = elementInfo.Name, TabElement = element};
                result.Add(tabInfo);
            }
            return result;
        }

        public BuildTreeViewTabElements AddElement(string name, TreeViewNetwork.DisplayType displayType)
        {
            m_ElementInfo.Add(new ElementInfo{Name = name, DisplayType = displayType});
            return this;
        }
        
        public BuildTreeViewTabElements AddElement(TreeViewNetwork.DisplayType displayType)
        {
            return AddElement(displayType.ToString(), displayType);
        }
    }
}