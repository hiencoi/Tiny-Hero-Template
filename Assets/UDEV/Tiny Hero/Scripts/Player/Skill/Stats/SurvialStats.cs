using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    [CreateAssetMenu(fileName = "Survival", menuName = "UDEV/" + GameConsts.PROJECT_NAME + "/Create Skill Stats/Survival")]
    public class SurvialStats : SkillStats
    {
        [Header("Skill Settings: ")]

        public float recoveryRate;
        [Header("Skill Upgrade Settings: ")]
        public float recoveryRateUp;

        protected override void CreateInfos(bool isPassive = false)
        {
            base.CreateInfos(true);

            Dictionary<string, string> pro_01 = new Dictionary<string, string>();

            pro_01.Add(recoveryRate.ToString() + " HP/s", "+" + (recoveryRateUp * m_infoLevel));

            m_infos.Add("HP Recover", pro_01);
        }

        public override void UpgradeSkill()
        {
            recoveryRate += recoveryRateUp * level;

            CreateInfos();
        }
    }
}
