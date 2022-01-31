using System;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class GameObjectViewModel : ViewModelBase
    {
        public GameObjectViewModel(NetworkObjectIdentifier objectIdentifier, IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                GetName(objectIdentifier),
                string.Empty,
                "gameobject",
                onSelectedCallback)
        {
        }

        static string GetName(NetworkObjectIdentifier objectIdentifier)
        {
            return !string.IsNullOrWhiteSpace(objectIdentifier.Name)
                ? objectIdentifier.Name
                : objectIdentifier.NetworkId.ToString();
        }
    }
}