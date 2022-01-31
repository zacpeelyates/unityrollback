namespace Unity.Multiplayer.Tools.NetStats
{
    interface IResettable
    {
        bool ShouldResetOnDispatch { get; }

        void Reset();
    }
}