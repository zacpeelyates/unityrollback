#if UNITY_2021_2_OR_NEWER
using System;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;
using Unity.Profiling.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class NetworkDetailsViewController : ProfilerModuleViewController
    {
        INetStatSerializer m_Serializer;
        InternalView m_InternalView;
        string m_TabName;

        public NetworkDetailsViewController(ProfilerWindow profilerWindow, string tabName)
            : base(profilerWindow)
        {
            m_Serializer = new NetStatSerializer();
            m_TabName = tabName;
        }

        protected override VisualElement CreateView()
        {
            ProfilerWindow.SelectedFrameIndexChanged += OnSelectedFrameIndexChanged;

            m_InternalView = new InternalView();
            m_InternalView.ShowTab(m_TabName);
            m_InternalView.PopulateView(GetDataForFrame(Convert.ToInt32(ProfilerWindow.selectedFrameIndex)));
            return m_InternalView;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            ProfilerWindow.SelectedFrameIndexChanged -= OnSelectedFrameIndexChanged;
            base.Dispose(true);
        }

        void OnSelectedFrameIndexChanged(long selectedFrameIndex)
        {
            m_InternalView.PopulateView(GetDataForFrame(Convert.ToInt32(selectedFrameIndex)));
        }

        MetricCollection GetDataForFrame(int frameIndex)
        {
            using var frameData = ProfilerDriver.GetRawFrameDataView(frameIndex, 0);
            
            if (frameData == null || !frameData.valid)
            {
                return null;
            }

            var bytes =
                frameData.GetFrameMetaData<byte>(FrameInfo.NetworkProfilerGuid, FrameInfo.NetworkProfilerDataTag);

            if (bytes.Length > 0)
            {
                return m_Serializer.Deserialize(bytes);
            }

            return null;
        }
    }
}
#endif