using System;
using Unity.Multiplayer.Tools.NetworkProfiler.Editor;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    internal class TestRowData : IRowData
    {
        public TestRowData(
            string objectName,
            string typeName,
            long sent = 0,
            long received = 0,
            bool sentOverLocalConnection = false)
        {
            Name = objectName;
            Bytes = new BytesSentAndReceived(sent, received);
            SentOverLocalConnection = sentOverLocalConnection;
            TypeName = typeName;
        }
        public IRowData Parent => null;
        public string TreeViewPath => Name;
        public string Name { get; }
        public BytesSentAndReceived Bytes { get; }
        public bool SentOverLocalConnection { get; }
        public string TypeDisplayName { get; }
        public string TypeName { get; }
        public Action OnSelectedCallback { get; }
    }
}