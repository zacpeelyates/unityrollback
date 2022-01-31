using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class ColumnBarNetwork : VisualElement
    {
        public delegate void ColumnClicked(bool isAscending);

        enum Columns
        {
            Name,
            Type,
            BytesSent,
            BytesReceived,
        }

        public event ColumnClicked NameClickEvent;
        public event ColumnClicked TypeClickEvent;
        public event ColumnClicked BytesSentClickEvent;
        public event ColumnClicked BytesReceivedClickEvent;

        readonly ColumnBarState m_State;
        readonly Button m_NameImage;
        readonly Button m_TypeImage;
        readonly Button m_BytesSentImage;
        readonly Button m_BytesReceivedImage;

        static class VisualElementNames
        {
            internal const string NameColumnLabel = nameof(NameColumnLabel);
            internal const string TypeColumnLabel = nameof(TypeColumnLabel);
            internal const string BytesSentColumnLabel = nameof(BytesSentColumnLabel);
            internal const string BytesReceivedColumnLabel = nameof(BytesReceivedColumnLabel);
            
            internal const string NameDirectionImage = nameof(NameDirectionImage);
            internal const string TypeDirectionImage = nameof(TypeDirectionImage);
            internal const string BytesSentDirectionImage = nameof(BytesSentDirectionImage);
            internal const string BytesReceivedDirectionImage = nameof(BytesReceivedDirectionImage);
        }

        static class VisualTreeAssetPaths
        {
            public const string Column =
                "Packages/com.unity.multiplayer.tools/NetworkProfiler/Editor/Windows/Columns/ColumnBar.uxml";
        }

        public ColumnBarNetwork()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreeAssetPaths.Column);
            var root = tree.CloneTree();

            var nameLabel = root.Q<Label>(VisualElementNames.NameColumnLabel);
            nameLabel.RegisterCallback<ClickEvent>(OnNameClicked);
            
            var typeLabel = root.Q<Label>(VisualElementNames.TypeColumnLabel);
            typeLabel.RegisterCallback<ClickEvent>(OnTypeClicked);

            var bytesSentLabel = root.Q<Label>(VisualElementNames.BytesSentColumnLabel);
            bytesSentLabel.RegisterCallback<ClickEvent>(OnBytesSentClicked);

            var bytesReceivedLabel = root.Q<Label>(VisualElementNames.BytesReceivedColumnLabel);
            bytesReceivedLabel.RegisterCallback<ClickEvent>(OnBytesReceivedClicked);
            
            m_NameImage = root.Q<Button>(VisualElementNames.NameDirectionImage);
            m_NameImage.RegisterCallback<ClickEvent>(OnNameClicked);
            
            m_TypeImage = root.Q<Button>(VisualElementNames.TypeDirectionImage);
            m_TypeImage.RegisterCallback<ClickEvent>(OnTypeClicked);

            m_BytesSentImage = root.Q<Button>(VisualElementNames.BytesSentDirectionImage);
            m_BytesSentImage.RegisterCallback<ClickEvent>(OnBytesSentClicked);

            m_BytesReceivedImage = root.Q<Button>(VisualElementNames.BytesReceivedDirectionImage);
            m_BytesReceivedImage.RegisterCallback<ClickEvent>(OnBytesReceivedClicked);

            m_State = new ColumnBarState();

            Add(root);
            
            m_NameImage.visible = false;
            m_TypeImage.visible = false;
            m_BytesSentImage.visible = false;
            m_BytesReceivedImage.visible = false;
        }

        static float GetImageAngle(bool isAscending) => isAscending ? 0 : 180;

        void UpdateDirectionUI(Columns column)
        {
            m_NameImage.visible = false;
            m_TypeImage.visible = false;
            m_BytesSentImage.visible = false;
            m_BytesReceivedImage.visible = false;
            switch (column)
            {
                case Columns.Name:
                    m_NameImage.visible = true;
                    break;
                case Columns.Type:
                    m_TypeImage.visible = true;
                    break;
                case Columns.BytesSent:
                    m_BytesSentImage.visible = true;
                    break;
                case Columns.BytesReceived:
                    m_BytesReceivedImage.visible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(column), column, null);
            }

            m_NameImage.SetRotate(GetImageAngle(m_State.IsNameAscending));
            m_TypeImage.SetRotate(GetImageAngle(m_State.IsTypeAscending));
            m_BytesSentImage.SetRotate(GetImageAngle(m_State.IsBytesSentAscending));
            m_BytesReceivedImage.SetRotate(GetImageAngle(m_State.IsBytesReceivedAscending));
        }

        void OnNameClicked(ClickEvent clickEvent)
        {
            var isAscending = m_State.ToggleNameSortDirection();
            UpdateDirectionUI(Columns.Name);
            NameClickEvent?.Invoke(isAscending);
        }

        void OnTypeClicked(ClickEvent clickEvent)
        {
            var isAscending = m_State.ToggleTypeSortDirection();
            UpdateDirectionUI(Columns.Type);
            TypeClickEvent?.Invoke(isAscending);
        }

        void OnBytesSentClicked(ClickEvent clickEvent)
        {
            var isAscending = m_State.ToggleBytesSentSortDirection();
            UpdateDirectionUI(Columns.BytesSent);
            BytesSentClickEvent?.Invoke(isAscending);
        }

        void OnBytesReceivedClicked(ClickEvent clickEvent)
        {
            var isAscending = m_State.ToggleBytesReceivedSortDirection();
            UpdateDirectionUI(Columns.BytesReceived);
            BytesReceivedClickEvent?.Invoke(isAscending);
        }
    }
}