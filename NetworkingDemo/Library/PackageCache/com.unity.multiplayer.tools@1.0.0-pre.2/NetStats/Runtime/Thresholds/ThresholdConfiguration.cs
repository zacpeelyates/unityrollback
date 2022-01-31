using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
    class ThresholdConfiguration<TStat, TValue> : IThresholdConfiguration
    {
        readonly Func<TStat, TValue> m_ValueProvider;
        readonly Func<TValue, bool> m_Condition;

        public ThresholdConfiguration(Func<TStat, TValue> valueProvider, Func<TValue, bool> condition)
        {
            m_ValueProvider = valueProvider;
            m_Condition = condition;
        }

        public bool IsConditionMet(IMetric networkStat)
        {
            try
            {
                return networkStat is TStat stat
                       && m_Condition.Invoke(m_ValueProvider.Invoke(stat));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to evaluate threshold condition: '{ex.Message}'.");
                
                return false;
            }
        }
    }
}