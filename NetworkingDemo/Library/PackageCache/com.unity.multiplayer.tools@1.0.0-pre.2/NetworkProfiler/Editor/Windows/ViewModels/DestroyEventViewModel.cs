using System;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{    internal class DestroyEventViewModel : ViewModelBase
     {
         public DestroyEventViewModel(IRowData parent, Action onSelectedCallback = null)
             : base(
                 parent,
                 $"{MetricType.ObjectDestroyed.GetDisplayNameString()}",
                 MetricType.ObjectDestroyed,
                 onSelectedCallback)
         {
         }
     }

}