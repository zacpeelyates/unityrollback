using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    internal struct ServerLogEvent : INetworkMetricEvent
    {
        public ServerLogEvent(ConnectionInfo connection, LogLevel logLevel, long bytesCount)
        {
            Connection = connection;
            LogLevel = logLevel;
            BytesCount = bytesCount;
        }

        public ConnectionInfo Connection { get; }

        public LogLevel LogLevel { get; }

        public long BytesCount { get; }
    }

    // These must stay in sync with NetworkLog.LogType in MLAPI
    internal enum LogLevel
    {
        Info,
        Warning,
        Error,
        None
    }
}