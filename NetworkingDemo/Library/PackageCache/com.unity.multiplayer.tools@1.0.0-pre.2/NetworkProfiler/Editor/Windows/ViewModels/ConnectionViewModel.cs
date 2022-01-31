using System;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class ConnectionViewModel : ViewModelBase
    {
        const string ServerName = "Server";
        const string ClientNamePrefix = "Client";

        public ConnectionViewModel(ConnectionInfo connectionInfo, ConnectionInfo localConnection, Action onSelectedCallback = null)
            : base(
                parent: null,
                name: GetName(connectionInfo),
                typeDisplayName: string.Empty,
                typeName: "connection",
                onSelectedCallback,
                connectionInfo,
                localConnection)
        {
        }

        static string GetName(ConnectionInfo connectionInfo)
        {
            return connectionInfo.Id == 0
                ? ServerName
                : $"{ClientNamePrefix} {connectionInfo.Id}";
        }
    }
}
