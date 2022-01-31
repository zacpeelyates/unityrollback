#if UNITY_2021_2_OR_NEWER
using System;
using Unity.Profiling.Editor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    [Serializable]
    [ProfilerModuleMetadata(Strings.GameObjectsProfilerModuleName)]
    class NetcodeObjectsProfilerModule : ProfilerModule
    {
        public NetcodeObjectsProfilerModule()
            : base(ProfilerModuleDefinitions.ObjectsProfilerModule.CountersAsDescriptors()) {}
        
        public override ProfilerModuleViewController CreateDetailsViewController()
            => new NetworkDetailsViewController(ProfilerWindow, TabNames.Activity);
    }
}
#endif
