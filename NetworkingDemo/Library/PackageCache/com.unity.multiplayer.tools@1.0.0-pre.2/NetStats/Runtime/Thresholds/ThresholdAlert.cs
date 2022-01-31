using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    readonly struct ThresholdAlert
    {
        public ThresholdAlert(IMetric metric, IThresholdConfiguration thresholdConfiguration)
        {
            Metric = metric;
            ThresholdConfiguration = thresholdConfiguration;
        }

        public IMetric Metric { get; }

        public IThresholdConfiguration ThresholdConfiguration { get; }
    }
}