using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    [CreateAssetMenu(fileName = "SupporterCall", menuName = "UDEV/" + GameConsts.PROJECT_NAME + "/Create Skill Stats/Supporter Call")]
    public class SupporterCallStats : SkillStats
    {
        [Header("Skill Settings: ")]
        public float damage;
        [Range(0f, 1f)]
        public float attackRate;
        public float fireRate;

        [Header("Bonus To Player Settings:")]
        public float healthBonus;
        public float damageBonus;

        [Header("Skill Upgrade Settings:")]
        public float damageUp;
        public float attackRateUp;
        public float fireRateUp;
        public float damageBonusUp;
        public float healthBonusUp;

        protected override void CreateInfos(bool isPassive = false)
        {
            base.CreateInfos(true);

            Dictionary<string, string> pro_01 = new Dictionary<string, string>();

            Dictionary<string, string> pro_02 = new Dictionary<string, string>();

            Dictionary<string, string> pro_03 = new Dictionary<string, string>();

            pro_01.Add(damage.ToString(), "+" + (damageUp * m_infoLevel));

            pro_02.Add(damageBonus.ToString(), "+" + (damageBonusUp * m_infoLevel));

            pro_03.Add(healthBonus.ToString(), "+" + (healthBonusUp * m_infoLevel));

            m_infos.Add("Own Damage", pro_01);

            m_infos.Add("Damage Bonus", pro_02);

            m_infos.Add("HP Bonus", pro_03);
        }

        public override void UpgradeSkill()
        {
            level++;
            damage += damageUp * level;
            attackRate += attackRateUp * level;
            attackRate = Mathf.Clamp(attackRate, 0f, 1f);
            fireRate -= fireRateUp * level;
            fireRate = Mathf.Clamp(fireRate, 0, fireRate);
            damageBonus += damageBonusUp * level;
            healthBonus += healthBonusUp * level;

            CreateInfos();
        }
    }
}
