using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    ///  Details view row entry that can be shared between the list and tree views
    internal class DetailsViewRow : VisualElement
    {
        private Label m_NameLabel;
        private Label m_TypeLabel;
        private Label m_BytesSentLabel;
        private Label m_BytesReceivedLabel;

        // Color values from here: https://docs.unity3d.com/2020.2/Documentation/Manual/UIE-USS-UnityVariables.html
        // and yes, we're planning to switch to USS styling because this hardcoded RGB isn't great
        private static readonly Color k_defaultTextColor_LightTheme = new Color(9.0f / 256, 9f / 256, 9f / 256);
        private static readonly Color k_defaultTextColor_DarkTheme = new Color(210f / 256, 210f / 256, 210f / 256);

        public DetailsViewRow()
        {
            m_NameLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    width = DetailsViewColumnLengths.Left,
                    overflow = Overflow.Hidden,
                    textOverflow = TextOverflow.Ellipsis,
                    unityTextOverflowPosition = TextOverflowPosition.End,
                },
            };
            m_TypeLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleRight,
                    width = DetailsViewColumnLengths.Middle,
                    maxWidth = DetailsViewColumnLengths.Middle,
                    minWidth = DetailsViewColumnLengths.Middle,
                },
            };
            m_BytesSentLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleRight,
                    width = DetailsViewColumnLengths.Middle,
                    maxWidth = DetailsViewColumnLengths.Middle,
                    minWidth = DetailsViewColumnLengths.Middle,
                },
            };
            m_BytesReceivedLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleRight,
                    width = DetailsViewColumnLengths.Right,
                    maxWidth = DetailsViewColumnLengths.Right,
                    minWidth = DetailsViewColumnLengths.Right,
                },
            };

            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1f;
            style.flexShrink = 0f;
            style.flexBasis = 0f;

            Add(m_NameLabel);
            Add(m_TypeLabel);
            Add(m_BytesSentLabel);
            Add(m_BytesReceivedLabel);
        }

        public void BindItem(ITreeViewItem item)
        {
            var rowDataItem = item as TreeViewItem<IRowData>;
            if (rowDataItem == null)
            {
                return;
            }
            var rowData = rowDataItem.data;

            m_NameLabel.text = rowData.Name;
            m_TypeLabel.text = rowData.TypeDisplayName;
            m_BytesSentLabel.text =
                FormattingUtil.FormatBytesForDetailsView(rowData.Bytes.Sent);
            m_BytesReceivedLabel.text =
                FormattingUtil.FormatBytesForDetailsView(rowData.Bytes.Received);

            var isDarkTheme = EditorGUIUtility.isProSkin;

            var textColorBase = isDarkTheme
                ? new StyleColor(k_defaultTextColor_DarkTheme)
                : new StyleColor(k_defaultTextColor_LightTheme);

            m_NameLabel.style.color = textColorBase;
            m_TypeLabel.style.color = textColorBase;

            var textColorBytes = rowData.SentOverLocalConnection
                ? new StyleColor(Color.gray)
                : textColorBase;

            m_BytesSentLabel.style.color = textColorBytes;
            m_BytesReceivedLabel.style.color = textColorBytes;

            // The tooltip text depends on the theme because it describes the significance
            // of text colors that vary based on the theme
            var bytesTooltipText = isDarkTheme
                ? Tooltips.DetailsViewBytes_DarkTheme
                : Tooltips.DetailsViewBytes_LightTheme;

            m_BytesSentLabel.tooltip = bytesTooltipText;
            m_BytesReceivedLabel.tooltip = bytesTooltipText;

            userData = item;
        }
    }
}