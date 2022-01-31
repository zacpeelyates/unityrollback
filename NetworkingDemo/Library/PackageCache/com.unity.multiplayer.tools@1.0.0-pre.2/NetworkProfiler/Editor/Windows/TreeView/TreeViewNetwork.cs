using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class TreeViewNetwork : VisualElement
    {
        public enum DisplayType
        {
            Messages,
            Activity,
        }

        TreeView m_InnerTreeView;
        TreeModel m_TreeModel;
        VisualElement m_TreeViewContainer;
        SortDirection m_SortDirection;
        DisplayType m_DisplayType;

        public TreeViewNetwork(TreeModel treeModel)
        {
            InitializeStyling();
            m_TreeModel = treeModel;
        }

        public void Show(DisplayType displayType)
        {
            m_DisplayType = displayType;
            if (HasConnections())
            {
                BuildTreeView(SortDirection.NameAscending);
            }
        }

        void BuildTreeView(SortDirection sort)
        {
            var rootItems = SortAndStructureData(sort, m_TreeModel);
            UpdateTreeView(rootItems);
        }

        static List<ITreeViewItem> SortAndStructureData(SortDirection sort, TreeModel tree)
        {
            tree.SortChildren(sort);
            return CreateTreeViewItemsFromTreeData(tree);
        }

        bool HasConnections()
        {
            return m_TreeModel.Children.Count > 0;
        }

        void InitializeStyling()
        {
            style.fontSize = 14;
            style.flexDirection = FlexDirection.Row;

            this.StretchToParentSize();
        }

        void UpdateTreeView(IList<ITreeViewItem> rootItems)
        {
            m_InnerTreeView?.RemoveFromHierarchy();
            m_InnerTreeView = new TreeView(rootItems, 20, MakeItem, BindItem);

            foreach (var item in rootItems)
            {
                SetExpandedStateRecursive(m_InnerTreeView, (TreeViewItem<IRowData>)item);
                SetSelectedStateRecursive(m_InnerTreeView, (TreeViewItem<IRowData>)item);
            }

            m_InnerTreeView.onExpandedStateChanged += UpdateFoldoutState;
            m_InnerTreeView.onItemsChosen += OnItemsChosen;
            m_InnerTreeView.onSelectionChange += OnSelectionChange;
            InitializeStylingTreeView(m_InnerTreeView);
            AddTreeView(m_InnerTreeView);
        }

        void OnItemsChosen(IEnumerable<ITreeViewItem> items)
        {
            foreach (var item in items)
            {
                var itemWithRow = item as TreeViewItem<IRowData>;
                itemWithRow?.data.OnSelectedCallback?.Invoke();
            }
        }

        void OnSelectionChange(IEnumerable<ITreeViewItem> items)
        {
            var locatorList = items
                .OfType<TreeViewItem<IRowData>>()
                .Select(t => t.data.TreeViewPath).ToList();

            DetailsViewPersistentState.SetSelected(locatorList);
        }

        static void InitializeStylingTreeView(TreeView treeView)
        {
            treeView.selectionType = SelectionType.Multiple;

            treeView.style.flexGrow = 1f;
            treeView.style.flexShrink = 0f;
            treeView.style.flexBasis = 0f;
        }

        static void SetExpandedStateRecursive(TreeView treeView, TreeViewItem<IRowData> item)
        {
            var expandedState = DetailsViewPersistentState.IsFoldedOut(item.data.TreeViewPath);
            if (expandedState)
            {
                treeView.ExpandItem(item.id);
            }
            else
            {
                treeView.CollapseItem(item.id);
            }

            if (item.children != null)
            {
                foreach (var child in item.children)
                {
                    SetExpandedStateRecursive(treeView, (TreeViewItem<IRowData>)child);
                }
            }
        }

        static void SetSelectedStateRecursive(TreeView treeView, TreeViewItem<IRowData> item)
        {
            var selectedState = DetailsViewPersistentState.IsSelected(item.data.TreeViewPath);
            if (selectedState)
            {
                treeView.AddToSelection(item.id);
            }
            else
            {
                treeView.RemoveFromSelection(item.id);
            }

            if (item.children != null)
            {
                foreach (var child in item.children)
                {
                    SetSelectedStateRecursive(treeView, (TreeViewItem<IRowData>)child);
                }
            }
        }

        void UpdateFoldoutState(int id, bool state)
        {
            var item = m_InnerTreeView.FindItem(id) as TreeViewItem<IRowData>;
            var locator = item.data.TreeViewPath;
            DetailsViewPersistentState.SetFoldout(locator.ToString(), state);
        }

        void AddTreeView(TreeView treeView)
        {
            m_TreeViewContainer?.RemoveFromHierarchy();
            m_TreeViewContainer = new VisualElement {name = "TreeView Container"};
            m_TreeViewContainer.style.flexGrow = 1f;
            m_TreeViewContainer.style.flexShrink = 0f;
            m_TreeViewContainer.style.flexBasis = 0f;

            m_TreeViewContainer.Add(treeView);

            Add(m_TreeViewContainer);
        }

        static VisualElement MakeItem()
        {
            return new DetailsViewRow();
        }

        static void BindItem(VisualElement element, ITreeViewItem item)
        {
            (element as DetailsViewRow)?.BindItem(item);
        }

        static List<ITreeViewItem> CreateTreeViewItemsFromTreeData(TreeModel tree)
        {
            var nextId = 0;
            return tree.Children.Select(child => CreateTreeViewItemsRecursive(child, ref nextId)).ToList();
        }

        static ITreeViewItem CreateTreeViewItemsRecursive(TreeModelNode node, ref int incrementalId)
        {
            var item = new TreeViewItem<IRowData>(incrementalId++, node.RowData);
            foreach (var child in node.Children)
            {
                item.AddChild(CreateTreeViewItemsRecursive(child, ref incrementalId));
            }

            return item;
        }

        public void NameSort(bool isAscending)
        {
            if (!HasConnections())
            {
                return;
            }

            var sort = isAscending
                ? SortDirection.NameAscending
                : SortDirection.NameDescending;
            BuildTreeView(sort);
        }

        public void TypeSort(bool isAscending)
        {
            if (!HasConnections())
            {
                return;
            }

            var sort = isAscending
                ? SortDirection.TypeAscending
                : SortDirection.TypeDescending;
            BuildTreeView(sort);
        }

        public void BytesSentSort(bool isAscending)
        {
            if (!HasConnections())
            {
                return;
            }

            var sort = isAscending
                ? SortDirection.BytesSentAscending
                : SortDirection.BytesSentDescending;
            BuildTreeView(sort);
        }

        public void BytesReceivedSort(bool isAscending)
        {
            if (!HasConnections())
            {
                return;
            }

            var sort = isAscending
                ? SortDirection.BytesReceivedAscending
                : SortDirection.BytesReceivedDescending;
            BuildTreeView(sort);
        }

        public void RefreshSelected()
        {
            if (m_InnerTreeView != null)
            {
                BuildTreeView(m_SortDirection);
            }
        }
    }
}
