using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class SkillsManager : MonoBehaviour
    {
        List<Skill> m_skills;
        List<Skill> m_ActiveSkills;
        Skill m_skillTriggered;

        public List<SkillSlot> slots = new List<SkillSlot>();

        public List<Skill> Skills { get => m_skills; set => m_skills = value; }
        public bool HasAnySkillTriggered { get => m_hasAnySkillTriggered; set => m_hasAnySkillTriggered = value; }
        public Skill SkillTriggered { get => m_skillTriggered; set => m_skillTriggered = value; }
        public List<Skill> ActiveSkills { get => m_ActiveSkills; set => m_ActiveSkills = value; }

        bool m_hasAnySkillTriggered;

        /// <summary>
        /// Initialize all skill
        /// </summary>
        /// <param name="aiController">Player Script</param>
        public void InitializeSkills(Player player)
        {
            if (HasAnySkillSlot())
            {
                m_skills = new List<Skill>();

                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i] != null && slots[i].skill && slots[i].playerLevel <= player.stats.level)
                    {
                        Skill skillClone = Instantiate(slots[i].skill, Vector3.zero, Quaternion.identity);

                        skillClone.SkillInit(player);

                        skillClone.animState = slots[i].animState;

                        skillClone.transform.SetParent(transform);

                        m_skills.Add(skillClone);
                    }
                }
            }
        }

        public void AutoRunSkillsTrigger()
        {
            if (m_ActiveSkills != null && m_ActiveSkills.Count > 0)
            {
                foreach (Skill skill in m_ActiveSkills)
                {
                    if (skill)
                        skill.AutoRunTrigger();
                }
            }
        }

        public void Recover()
        {
            if(m_ActiveSkills != null && m_ActiveSkills.Count > 0)
            {
                foreach(Skill skill in m_ActiveSkills)
                {
                    if (skill)
                        skill.Recover();
                }
            }
        }

        public void TriggerOnAnim()
        {
            if (m_skillTriggered != null)
                m_skillTriggered.OnAnimTrigger();
        }

        /// <summary>
        /// Excute all skill unlocked
        /// </summary>
        public void TriggerSkills()
        {
            if (m_skills != null && m_skills.Count > 0)
            {
                foreach (Skill sk in m_skills)
                {
                    if (sk != null && sk.isUnlocked)
                    {
                        sk.Trigger();
                    }
                }
            }
        }

        /// <summary>
        /// Reset all skill
        /// </summary>
        public void ResetSkills()
        {
            if (m_skills != null && m_skills.Count > 0)
            {
                foreach (Skill sk in m_skills)
                {
                    if (sk != null && sk.isUnlocked)
                    {
                        sk.FinalReset();
                    }
                }
            }
        }

        /// <summary>
        /// Get skill by name in skill list
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Ability</returns>
        public Skill GetSkillByName(string name)
        {
            if (m_skills != null && m_skills.Count > 0)
            {
                foreach (Skill sk in m_skills)
                {
                    if (sk != null &&
                        string.Compare(sk.name, name) == 0)

                        return sk;
                }
            }
            return null;
        }

        /// <summary>
        /// Get skill by id in skill list
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Ability</returns>
        public Skill GetSkillById(string id)
        {
            if (m_skills != null && m_skills.Count > 0)
            {
                foreach (Skill sk in m_skills)
                {
                    if (sk != null &&
                        string.Compare(sk.id, id) == 0)

                        return sk;
                }
            }
            return null;
        }

        /// <summary>
        /// Get skill by index in skill list
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Ability</returns>
        public Skill GetSkillByIndex(int index)
        {
            if (m_skills != null && m_skills.Count > 0)
            {
                for (int i = 0; i < m_skills.Count; i++)
                {
                    if (m_skills[i] != null && index == i)

                        return m_skills[i];
                }
            }

            return null;
        }

        public List<Skill> GetUnlockedSkills()
        {
            List<Skill> unlockedSkills = new List<Skill>();

            if (m_skills != null && m_skills.Count > 0)
            {
                foreach (Skill sk in m_skills)
                {
                    if (sk != null && sk.isUnlocked)
                        unlockedSkills.Add(sk);
                }
            }
            return unlockedSkills;
        }

        /// <summary>
        /// Get all skill name in skill list
        /// </summary>
        /// <returns>List of skill names</returns>
        public string[] GetSkillNames()
        {
            if (m_skills != null && m_skills.Count > 0)
            {
                string[] names = new string[m_skills.Count];

                for (int i = 0; i < slots.Count; i++)
                {
                    if (m_skills[i] != null)
                        names[i] = m_skills[i].name;
                }
                return names;
            }
            return null;
        }

        /// <summary>
        /// Get all skill id in skill list
        /// </summary>
        /// <returns>List of skill ids</returns>
        public string[] GetSkillIds()
        {
            if (HasAnySkillSlot())
            {
                string[] ids = new string[slots.Count];

                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i] != null && slots[i].skill != null)
                        ids[i] = slots[i].skill.id;
                }
                return ids;
            }
            return null;
        }

        public List<Skill> GetUlockedSkills(Player player)
        {
            List<Skill> unlocked = new List<Skill>();

            if (HasAnySkillSlot())
            {
                foreach (SkillSlot s in slots)
                {
                    if (s != null && s.playerLevel == player.stats.level)
                    {
                        unlocked.Add(s.skill);
                    }
                }
            }

            return unlocked;
        }

        /// <summary>
        /// Lock all skill except the skill define by id
        /// </summary>
        /// <param name="id"></param>
        public void LockSkillsExcept(string id)
        {
            if(m_skills != null && m_skills.Count > 0)
            {
                foreach(Skill sk in m_skills)
                {
                    if(sk)
                    {
                        if (string.Compare(sk.id, id) == 0)
                            sk.isUnlocked = false;
                        else
                            sk.isUnlocked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Check player have enough level to take skill
        /// </summary>
        /// <param name="skillSlot">Skill Slot</param>
        /// <param name="player">Player Script</param>
        /// <returns></returns>
        public bool IsCanTakeSkill(SkillSlot skillSlot, int level)
        {
            return level >= skillSlot.playerLevel;
        }

        public SkillSlot GetSlot(string id)
        {
            if (HasAnySkillSlot())
            {
                foreach (SkillSlot ab in slots)
                {
                    if (ab != null && ab.skill != null)
                    {
                        if (string.Compare(ab.skill.id, id) == 0)
                        {
                            return ab;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Check skill manager have any skill slot
        /// </summary>
        public bool HasAnySkillSlot()
        {
            return slots != null && slots.Count > 0;
        }
    }
}
