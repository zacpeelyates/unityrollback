using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class TreeModel
    {
        public IReadOnlyList<TreeModelNode> Children => m_Children;
        List<TreeModelNode> m_Children = new List<TreeModelNode>();
        
        public void SortChildren(SortDirection direction)
        {
            m_Children.Sort((a,b) => RowDataSorting.SortOperation(a.RowData, b.RowData, direction));
            foreach (var child in Children)
            {
                child.SortChildren(direction);
            }
        }

        public void AddChild(TreeModelNode node)
        {
            m_Children.Add(node);
        } 
    }
}
