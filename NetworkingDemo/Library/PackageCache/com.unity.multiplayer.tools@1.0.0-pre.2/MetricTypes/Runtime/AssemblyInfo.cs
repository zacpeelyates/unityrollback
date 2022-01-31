using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Netcode.Runtime")]
[assembly: InternalsVisibleTo("Unity.Netcode.RuntimeTests")]
[assembly: InternalsVisibleTo("Unity.Netcode.EditorTests")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.NetworkProfiler.Runtime")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.NetworkProfiler.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.NetworkProfiler.Tests.Editor")]
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("TestProject.ToolsIntegration.RuntimeTests")]
[assembly: InternalsVisibleTo("TestProject.EditorTests")]
[assembly: InternalsVisibleTo("TestProject.RuntimeTests")]
#endif
