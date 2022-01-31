using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class InternalView : VisualElement
    {
        const bool k_ShowTabDropdown = false;
        const int k_TabDropdownWidth = 120;

        readonly List<TabInfo> m_TabInfos;
        MetricCollection m_MetricsCollection;
        string m_CurrentTab;
        TabElement m_CurrentTabElement;
        VisualElement m_CustomizableToolbarContainer;
        ToolbarMenu m_TabsDropdown;
        Toolbar m_Toolbar;

        public InternalView()
        {
            m_TabInfos = BuildTreeViewTabElements.CreateTabElements()
                .AddElement(TreeViewNetwork.DisplayType.Activity)
                .AddElement(TreeViewNetwork.DisplayType.Messages)
                .Build();

            SetupInitialUIElements();

            m_CurrentTab = m_TabInfos[0].Name;

            PopulateView(null);

            style.flexGrow = 1;
        }

        public void ShowTab(string tabName)
        {
            if (!TryGetTabInfo(tabName, out TabInfo tabInfo))
            {
                Debug.LogError($"Could not find tab with name {tabName}");
                return;
            }

            m_CurrentTabElement?.Hide();
            m_CurrentTabElement = tabInfo.TabElement;
            m_CurrentTabElement.UpdateMetrics(m_MetricsCollection);
            m_CurrentTabElement.Show();

            m_CustomizableToolbarContainer.Clear();
            m_CurrentTabElement.CustomizeToolbar(m_CustomizableToolbarContainer);

            m_TabsDropdown.text = tabName;
            m_CurrentTab = tabName;
        }

        bool TryGetTabInfo(string tabName, out TabInfo tabInfo)
        {
            foreach (var tab in m_TabInfos)
            {
                if (tab.Name == tabName)
                {
                    tabInfo = tab;
                    return true;
                }
            }

            tabInfo = default;
            return false;
        }

        public void PopulateView(MetricCollection metricCollection)
        {
            CacheMetrics(metricCollection);
            ShowTab(m_CurrentTab);
        }

        void SetupInitialUIElements()
        {
            m_Toolbar = new Toolbar();
            Add(m_Toolbar);
            
            m_TabsDropdown = new ToolbarMenu();
            m_TabsDropdown.style.width = k_TabDropdownWidth;
            m_Toolbar.Add(m_TabsDropdown);

            if (!k_ShowTabDropdown)
            {
                m_TabsDropdown.style.display = DisplayStyle.None;
            }

            m_CustomizableToolbarContainer = new VisualElement();
            m_CustomizableToolbarContainer.style.flexGrow = 1f;
            m_CustomizableToolbarContainer.style.flexDirection = FlexDirection.Row;
            m_Toolbar.Add(m_CustomizableToolbarContainer);

            foreach (var tab in m_TabInfos)
            {
                m_TabsDropdown.menu.AppendAction(tab.Name, SelectTabFromDropdown, IsTabSelected);
                Add(tab.TabElement);
                tab.TabElement.Hide();
            }
        }

        void CacheMetrics(MetricCollection metricCollection)
        {
            m_MetricsCollection = metricCollection;
        }

        DropdownMenuAction.Status IsTabSelected(DropdownMenuAction arg)
        {
            return arg.name == m_CurrentTab
                ? DropdownMenuAction.Status.Checked
                : DropdownMenuAction.Status.Normal;
        }

        void SelectTabFromDropdown(DropdownMenuAction obj)
        {
            ShowTab(obj.name);
        }
    }
}