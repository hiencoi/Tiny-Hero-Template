using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    [CreateAssetMenu(fileName = "Hidden Power", menuName = "UDEV/" + GameConsts.PROJECT_NAME + "/Create Skill Stats/Hidden Power")]
    public class HiddenPowerStats : SkillStats
    {
        [Header("Skill Settings: ")]
        public float invincibleTime;
        public float damageExtra;
        public float healthRecovery;

        [Header("Skill Upgrade Settings: ")]
        public float invincibleTimeUp;
        public float dmExtraUp;
        public float hpRecoveryUp;

        protected override void CreateInfos(bool isPassive = false)
        {
            base.CreateInfos();

            Dictionary<string, string> pro_01 = new Dictionary<string, string>();

            Dictionary<string, string> pro_02 = new Dictionary<string, string>();

            Dictionary<string, string> pro_03 = new Dictionary<string, string>();

            pro_01.Add(invincibleTime.ToString() + "s", "+" + (invincibleTimeUp * m_infoLevel) + "s");

            pro_02.Add(damageExtra.ToString(), "+" + (dmExtraUp * m_infoLevel));

            pro_03.Add(healthRecovery.ToString() + " Hp/s", "+" + hpRecoveryUp * m_infoLevel);

            m_infos.Add("Invincible Time", pro_01);

            m_infos.Add("Damage Bonus", pro_02);

            m_infos.Add("HP Recover", pro_03);
        }

        public override void UpgradeSkill()
        {
            invincibleTime += invincibleTimeUp * level;
            damageExtra += dmExtraUp * level;
            healthRecovery += hpRecoveryUp * level;

            CreateInfos();
        }
    }
}
