using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class TreeModelNode
    {
        List<TreeModelNode> m_Children;

        public TreeModelNode(IRowData rowData)
        {
            RowData = rowData;
        }

        internal IRowData RowData { get; }

        internal IReadOnlyList<TreeModelNode> Children =>
            m_Children ?? (IReadOnlyList<TreeModelNode>)Array.Empty<TreeModelNode>();

        internal TreeModelNode Parent { get; private set; }
        
        public void Add(TreeModelNode node) => AddChild(node);
        public void AddChild(TreeModelNode node)
        {
            m_Children ??= new List<TreeModelNode>();

            m_Children.Add(node);
            node.Parent = this;
        }

        public void AddChildren(IEnumerable<TreeModelNode> nodes)
        {
            m_Children ??= new List<TreeModelNode>();

            m_Children.AddRange(nodes);
            foreach (var node in nodes)
            {
                node.Parent = this;
            }
        }

        public void SortChildren(SortDirection direction)
        {
            if (m_Children != null)
            {
                m_Children.Sort((a,b) => RowDataSorting.SortOperation(a.RowData, b.RowData, direction));
                for (var i = 0; i < m_Children.Count; ++i)
                {
                    m_Children[i].SortChildren(direction);
                }
            }
        }
    }
}