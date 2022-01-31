namespace Unity.Multiplayer.Tools.NetStats
{
    interface IMetricObserver
    {
        void Observe(MetricCollection collection);
    }
}