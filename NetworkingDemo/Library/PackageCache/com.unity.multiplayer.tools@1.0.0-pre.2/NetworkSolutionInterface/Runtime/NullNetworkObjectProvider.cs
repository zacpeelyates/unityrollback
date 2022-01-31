using UnityEngine;

namespace Unity.Multiplayer.Tools
{
    class NullNetworkObjectProvider : INetworkObjectProvider
    {
        Object INetworkObjectProvider.GetNetworkObject(ulong networkObjectId) => null;
    }
}