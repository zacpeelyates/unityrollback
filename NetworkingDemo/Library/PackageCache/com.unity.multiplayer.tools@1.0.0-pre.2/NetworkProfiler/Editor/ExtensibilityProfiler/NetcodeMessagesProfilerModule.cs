#if UNITY_2021_2_OR_NEWER
using System;
using Unity.Profiling.Editor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    [Serializable]
    [ProfilerModuleMetadata(Strings.MessagesProfilerModuleName)]
    class NetcodeMessagesProfilerModule : ProfilerModule
    {
        public NetcodeMessagesProfilerModule()
            : base(ProfilerModuleDefinitions.MessagesProfilerModule.CountersAsDescriptors()) {}
        
        public override ProfilerModuleViewController CreateDetailsViewController()
            => new NetworkDetailsViewController(ProfilerWindow, TabNames.Messages);
    }
}
#endif
