namespace Unity.Multiplayer.Tools.NetStats
{
    interface IMetricDispatcher
    {
        void RegisterObserver(IMetricObserver observer);

        void SetConnectionId(ulong connectionId);

        void Dispatch();
    }
}