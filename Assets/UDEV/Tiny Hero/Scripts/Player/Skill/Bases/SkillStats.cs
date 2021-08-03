using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class SkillStats : Stats
    {
        [Header("Base Settings: ")]
        public int maxLevel;
        public int level;
        public float cooldownTime;
        public float coolDownRate;

        [Header("Base Upgrade Settings: ")]
        public int skillPoints;
        public int skillPointsUp;
        public float coolDownUp;
        protected int m_infoLevel;

        protected Dictionary<string, Dictionary<string, string>> m_infos;

        public Dictionary<string, Dictionary<string, string>> Infos { get => m_infos;}

        protected virtual void CreateInfos(bool isPassive = false)
        {
            m_infoLevel = level + 1;

            m_infos = new Dictionary<string, Dictionary<string, string>>();

            Dictionary<string, string> pro_01 = new Dictionary<string, string>();

            if (!isPassive)
            {
                pro_01.Add(((cooldownTime * coolDownRate) * 100).ToString("F1") + "s", "-" + (coolDownUp * m_infoLevel) + "s");

                m_infos.Add("Cooldown Time", pro_01);
            }
        }

        public override void Init()
        {
            base.Init();
            CreateInfos();
        }

        public bool IsMaxLevel()
        {
            return level >= maxLevel;
        }

        public bool IsCanUpgrade(int point)
        {
            return point >= skillPoints;
        }

        public void Upgrade()
        {
            level++;

            cooldownTime -= coolDownUp * level;

            skillPoints += skillPointsUp * level;

            UpgradeSkill();
        }

        public virtual void UpgradeSkill()
        {

        }
    }
}
