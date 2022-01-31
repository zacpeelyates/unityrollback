using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal struct ProfilerModuleDefinition
    {
        public string Name;
        public string[] Counters;
    }
    
    internal static class ProfilerModuleDefinitions
    {
        static ProfilerCounters ProfilerCounters => ProfilerCounters.Instance;

        public static readonly IReadOnlyList<ProfilerModuleDefinition> Modules;

        static ProfilerModuleDefinitions()
        {
            Modules = new List<ProfilerModuleDefinition>()
            {
                MessagesProfilerModule,
                ObjectsProfilerModule
            };
        }
        
        internal static readonly ProfilerModuleDefinition ObjectsProfilerModule = new ProfilerModuleDefinition
        {
            Name = Strings.GameObjectsProfilerModuleName,
            Counters = new []
            {
                ProfilerCounters.rpc.Bytes.Sent, 
                ProfilerCounters.rpc.Bytes.Received,
                ProfilerCounters.networkVariableDelta.Bytes.Sent,
                ProfilerCounters.networkVariableDelta.Bytes.Received,
                ProfilerCounters.objectSpawned.Bytes.Sent,
                ProfilerCounters.objectSpawned.Bytes.Received,
                ProfilerCounters.objectDestroyed.Bytes.Sent,
                ProfilerCounters.objectDestroyed.Bytes.Received,
                ProfilerCounters.ownershipChange.Bytes.Sent,
                ProfilerCounters.ownershipChange.Bytes.Received,
            }
        };
        
        internal static readonly ProfilerModuleDefinition MessagesProfilerModule = new ProfilerModuleDefinition
        {
            Name = Strings.MessagesProfilerModuleName,
            Counters = new []
            {
                ProfilerCounters.totalBytes.Sent,
                ProfilerCounters.totalBytes.Received,
                ProfilerCounters.namedMessage.Bytes.Sent,
                ProfilerCounters.namedMessage.Bytes.Received,
                ProfilerCounters.unnamedMessage.Bytes.Sent,
                ProfilerCounters.unnamedMessage.Bytes.Received,
                ProfilerCounters.sceneEvent.Bytes.Sent,
                ProfilerCounters.sceneEvent.Bytes.Received,
            }
        };
    }
}