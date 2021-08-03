using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Class used for all stats where we want to be able to add/remove modifiers */

namespace UDEV
{
    [System.Serializable]
    public class Stat
    {
        public float baseValue;  // Starting value

        // List of modifiers that change the baseValue
        private List<float> m_modifiers = new List<float>();

        // Get the final value after applying modifiers
        public float GetValue()
        {
            float finalValue = baseValue;
            m_modifiers.ForEach(x => finalValue += x);
            return finalValue;
        }

        public int GetIntValue()
        {
            float finalValue = baseValue;
            m_modifiers.ForEach(x => finalValue += x);
            return Mathf.RoundToInt(finalValue);
        }

        public int GetIntBaseValue()
        {
            return Mathf.RoundToInt(baseValue);
        }

        // Add new modifier
        public void AddModifier(float modifier)
        {
            if (modifier != 0)
                m_modifiers.Add(modifier);
        }

        // Remove a modifier
        public void RemoveModifier(float modifier)
        {
            if (modifier != 0)
                m_modifiers.Remove(modifier);
        }

        public void RemoveModifiers()
        {
            m_modifiers.Clear();
        }
    }
}