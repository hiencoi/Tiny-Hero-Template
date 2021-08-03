using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {

    [CreateAssetMenu(fileName = "FreezeEnemies", menuName = "UDEV/" + GameConsts.PROJECT_NAME + "/Create Skill Stats/Freeze Enemies")]
    public class FreezeEnemiesStats : SkillStats
    {
        [Header("Skill Settings: ")]
        public float freezeTime;

        [Header("Skill Upgrade Settings:")]
        public float freezeTimeUp;

        protected override void CreateInfos(bool isPassive = false)
        {
            base.CreateInfos();

            Dictionary<string, string> pro_01 = new Dictionary<string, string>();

            pro_01.Add(freezeTime.ToString() + "s", "+" + (freezeTimeUp * m_infoLevel) + "s");

            m_infos.Add("Freeze Time", pro_01);
        }

        public override void UpgradeSkill()
        {
            freezeTime += freezeTimeUp * level;

            CreateInfos();
        }
    }
}
