using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    [CreateAssetMenu(fileName = "NEW_PLAYER_STATS", menuName = "UDEV/" + GameConsts.PROJECT_NAME + "/Create Player Stats")]
    public class PlayerStats : Stats
    {
        [Header("Base Attributes:")]
        public Stat health;

        [Header("LevelUp Settings:")]
        public int maxLevel;
        public int level;
        public float levelUpXp;
        public float xp;
        public int skillPoints;
        public float xpUp;
        public float hpUp;

        public override void SetStats()
        {
            m_stats = new Stat[] { health };
        }

        public void LevelUp()
        {
            while (xp >= levelUpXp)
            {
                level++;
                xp -= levelUpXp;
                skillPoints ++;
                levelUpXp += xpUp * level;
                health.baseValue += hpUp * level;
            }
        }

        public bool IsMaxLevel()
        {
            return level >= maxLevel;
        }

        public bool CanLevelUp()
        {
            return xp >= levelUpXp;
        }
    }
}
