using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UDEV.AI2D
{
    public class aiFeaturesManager : MonoBehaviour
    {
        //List of feature slot
        public List<aiFeatureSlot> featureSlots = new List<aiFeatureSlot>();

        //Variable to check feature enable or not
        bool m_enable = true;
        bool m_hasActiveFeature;
        aiFeature m_activeFeature;

        public bool Enable { get => m_enable; set => m_enable = value; }
        public bool HasActiveFeature { get => m_hasActiveFeature; set => m_hasActiveFeature = value; }
        public aiFeature ActiveFeature {
            get {
                if (HasAnyFeatureSlot())
                {
                    var proactiveFeatures = featureSlots.Where(
                        f => !f.isLocked && !f.feature.isPassive && f.feature.IsReady()
                        ).ToArray();

                    if (proactiveFeatures != null && proactiveFeatures.Length > 0)
                    {
                        int randomIdx = Random.Range(0, proactiveFeatures.Length);

                        if (proactiveFeatures[randomIdx] != null && proactiveFeatures[randomIdx].feature != null)
                            m_activeFeature = proactiveFeatures[randomIdx].feature;
                    }
                    else
                    {
                        m_activeFeature = null;
                    }
                }
                else
                {
                    m_activeFeature = null;
                }
                return m_activeFeature;
            }
            set => m_activeFeature = value;
        }

        /// <summary>
        /// Initialize data all of features
        /// </summary>
        public void InitializeFeatures(aiBase aiController)
        {
            if (HasAnyFeatureSlot())
            {
                foreach (aiFeatureSlot slot in featureSlots)
                {
                    if (slot.feature != null && !slot.isLocked)
                    {
                        slot.feature.Initialize(aiController);
                    }
                }
            }
        }

        public void TriggerOnAnim()
        {
            if (m_activeFeature != null)
                m_activeFeature.OnAnimTrigger();
        }

        /// <summary>
        /// Get feature by name in list of feature slot
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Feature</returns>
        public aiFeature GetFeatureByName(string name)
        {
            if (HasAnyFeatureSlot())
            {
                foreach (aiFeatureSlot slot in featureSlots)
                {
                    if (slot != null &&
                        slot.feature != null && !slot.isLocked &&
                        string.Compare(slot.name, name) == 0)

                        return slot.feature;
                }
            }
            return null;
        }

        /// <summary>
        /// Get feature by id in list of feature
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Feature</returns>
        public aiFeature GetFeatureById(string id)
        {
            if (HasAnyFeatureSlot())
            {
                foreach (aiFeatureSlot slot in featureSlots)
                {
                    if (slot != null && !slot.isLocked &&
                        slot.feature != null &&
                        string.Compare(slot.feature.id, id) == 0)

                        return slot.feature;
                }
            }
            return null;
        }

        /// <summary>
        /// Get feature by index in list of feature
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Feature</returns>
        public aiFeature GetFeatureByIndex(int index)
        {
            if (HasAnyFeatureSlot())
            {
                for (int i = 0; i < featureSlots.Count; i++)
                {
                    if (featureSlots[i].feature != null && !featureSlots[i].isLocked && index == i)

                        return featureSlots[i].feature;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all feature name of feature slot
        /// </summary>
        /// <returns>List of feature names</returns>
        public List<string> GetFeatureNames()
        {
            if (HasAnyFeatureSlot())
            {
                List<string> names = new List<string>();

                for (int i = 0; i < featureSlots.Count; i++)
                {
                    if (featureSlots[i] != null)

                        names.Add(featureSlots[i].name);
                }
                return names;
            }
            return null;
        }

        /// <summary>
        /// Get all feature id of feature slot
        /// </summary>
        /// <returns>List of feature ids</returns>
        public List<string> GetFeatureIds()
        {
            if (HasAnyFeatureSlot())
            {
                List<string> ids = new List<string>();

                for (int i = 0; i < featureSlots.Count; i++)
                {
                    if (featureSlots[i] != null && featureSlots[i].feature != null)

                        ids.Add(featureSlots[i].feature.id);
                }
                return ids;
            }
            return null;
        }

        /// <summary>
        /// Check feature exist in list of feature slot
        /// </summary>
        /// <param name="name">Feature Name</param>
        public bool IsFeatureExist(string name)
        {
            if (HasAnyFeatureSlot())
            {
                foreach (aiFeatureSlot ab in featureSlots)
                {
                    if (ab != null && string.Compare(ab.name, name) == 0)

                        return true;
                }
            }

            return false;
        }

        public void RemoveAllModifiers()
        {
            if (HasAnyFeatureSlot())
            {
                foreach (aiFeatureSlot ab in featureSlots)
                {
                    if (ab != null && ab.feature != null)
                        ab.feature.RemoveModifiers();
                }
            }
        }

        public void AddModifierToAll(int val)
        {
            if (HasAnyFeatureSlot())
            {
                foreach (aiFeatureSlot ab in featureSlots)
                {
                    if (ab != null && ab.feature != null)
                        ab.feature.damage.AddModifier(val);
                }
            }
        }

        /// <summary>
        /// Check feature manager have any feature slot
        /// </summary>
        public bool HasAnyFeatureSlot()
        {
            return featureSlots != null && featureSlots.Count > 0;
        }
    }
}
