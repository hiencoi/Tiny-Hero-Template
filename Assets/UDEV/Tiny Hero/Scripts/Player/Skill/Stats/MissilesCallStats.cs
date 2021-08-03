using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    [CreateAssetMenu(fileName = "MissilesCall", menuName = "UDEV/" + GameConsts.PROJECT_NAME + "/Create Skill Stats/Missiles Call")]
    public class MissilesCallStats : SkillStats
    {
        [Header("Skill Settings: ")]
        public int maxMissiles;
        public float damage;

        [Header("Skill Upgrade Settings: ")]
        public int missilesUp;
        public float damageUp;

        protected override void CreateInfos(bool isPassive = false)
        {
            base.CreateInfos();

            Dictionary<string, string> pro_01 = new Dictionary<string, string>();

            Dictionary<string, string> pro_02 = new Dictionary<string, string>();

            pro_01.Add(maxMissiles.ToString(), "+" + missilesUp);

            pro_02.Add(damage.ToString(), "+" + (damageUp * m_infoLevel));

            m_infos.Add("Fire Missiles", pro_01);

            m_infos.Add("Damage", pro_02);
        }

        public override void UpgradeSkill()
        {
            damage += damageUp * level;
            maxMissiles += missilesUp;

            CreateInfos();
        }
    }
}
