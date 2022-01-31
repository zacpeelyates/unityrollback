using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    [Serializable]
    class DetailsViewFoldoutState : ISerializationCallbackReceiver
    {
        [SerializeField]
        bool m_DefaultFoldoutStateExpanded = false;

        [SerializeField]
        List<string> m_FoldoutExceptionsSerialized = new List<string>();
        HashSet<string> m_FoldoutExceptions = new HashSet<string>();

        public bool IsFoldedOut(string locator)
        {
            if (m_DefaultFoldoutStateExpanded)
            {
                // If default is expanded, then exception means contracted
                return !m_FoldoutExceptions.Contains(locator);
            }
            
            return m_FoldoutExceptions.Contains(locator);
        }

        public void SetFoldout(string locator, bool isFoldedOut)
        {
            if (isFoldedOut)
            {
                Expand(locator);
            }
            else
            {
                Contract(locator);
            }
        }

        void Expand(string locator)
        {
            if (m_DefaultFoldoutStateExpanded)
            {
                m_FoldoutExceptions.Remove(locator);
            }
            else
            {
                m_FoldoutExceptions.Add(locator);
            }
        }

        void Contract(string locator)
        { 
            if (m_DefaultFoldoutStateExpanded)
            {
                m_FoldoutExceptions.Add(locator);
            }
            else
            {
                m_FoldoutExceptions.Remove(locator);
            }
        }

        public void SetFoldoutExpandAll()
        {
            m_FoldoutExceptions.Clear();
            m_DefaultFoldoutStateExpanded = true;
        }

        public void SetFoldoutContractAll()
        {
            m_FoldoutExceptions.Clear();
            m_DefaultFoldoutStateExpanded = false;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_FoldoutExceptionsSerialized.Clear();
            m_FoldoutExceptionsSerialized.AddRange(m_FoldoutExceptions);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_FoldoutExceptions.Clear();
            foreach (var value in m_FoldoutExceptionsSerialized)
            {
                m_FoldoutExceptions.Add(value);
            }
        }
    }
}
