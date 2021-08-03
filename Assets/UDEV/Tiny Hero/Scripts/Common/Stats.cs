using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV
{
    public class Stats : ScriptableObject
    {
        protected Stat[] m_stats;

        public virtual void SetStats()
        {

        }

        protected void ClearStats()
        {
            SetStats();

            if(m_stats != null && m_stats.Length > 0)
            {
                for (int i = 0; i < m_stats.Length; i++)
                {
                    if (m_stats[i] != null)
                        m_stats[i].RemoveModifiers();
                }
            }
        }

        public virtual void Init()
        {
            ClearStats();
        }
    }
}