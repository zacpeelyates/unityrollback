using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct SceneEventMetric : INetworkMetricEvent
    {
        public SceneEventMetric(ConnectionInfo connection, string sceneEventType, string sceneName, long bytesCount)
        {
            Connection = connection;
            SceneEventType = sceneEventType;
            SceneName = sceneName;
            BytesCount = bytesCount;
        }

        public ConnectionInfo Connection { get; }

        public string SceneEventType{ get; }

        public string SceneName { get; }

        public long BytesCount { get; }
    }
}