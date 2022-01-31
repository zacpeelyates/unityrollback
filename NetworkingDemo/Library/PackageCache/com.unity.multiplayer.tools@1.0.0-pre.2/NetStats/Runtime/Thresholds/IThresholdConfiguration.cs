namespace Unity.Multiplayer.Tools.NetStats
{
    interface IThresholdConfiguration
    {
        bool IsConditionMet(IMetric networkStat);
    }
}