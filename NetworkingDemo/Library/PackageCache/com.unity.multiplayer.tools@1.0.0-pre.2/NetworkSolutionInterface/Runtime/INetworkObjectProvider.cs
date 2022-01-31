using UnityEngine;

namespace Unity.Multiplayer.Tools
{
    interface INetworkObjectProvider
    {
        Object GetNetworkObject(ulong networkObjectId);
    }
}
