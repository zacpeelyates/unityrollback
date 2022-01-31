using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    [Serializable]
    internal class DetailsViewSelectedState : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> m_SelectedSerialized = new List<string>();
        HashSet<string> m_Selected = new HashSet<string>();

        [CanBeNull]
        [field: SerializeField]
        public string MostRecentlySelected { get; set; }

        public void SetSelected(IReadOnlyCollection<string> locators)
        {
            m_Selected.Clear();
            foreach (var locator in locators)
            {
                m_Selected.Add(locator);
            }
        }

        public bool IsSelected(string locator)
        {
            return m_Selected.Contains(locator);
        }

        public void OnBeforeSerialize()
        {
            m_SelectedSerialized.Clear();
            m_SelectedSerialized.AddRange(m_Selected);
        }

        public void OnAfterDeserialize()
        {
            m_Selected.Clear();
            foreach (var value in m_SelectedSerialized)
            {
                m_Selected.Add(value);
            }
        }
    }
}
