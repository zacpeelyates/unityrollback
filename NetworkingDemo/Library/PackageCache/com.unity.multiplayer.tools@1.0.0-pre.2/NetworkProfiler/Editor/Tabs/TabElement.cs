using Unity.Multiplayer.Tools.NetStats;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    abstract class TabElement : VisualElement
    {
        protected TabElement()
        {
            style.flexGrow = 1;
        }

        public abstract void UpdateMetrics(MetricCollection metricCollection);
        
        public abstract void Show();
        public abstract void Hide();
        public virtual void CustomizeToolbar(VisualElement container) { }
    }
}
