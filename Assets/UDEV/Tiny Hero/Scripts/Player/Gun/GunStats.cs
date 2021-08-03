using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    [CreateAssetMenu(fileName = "NEW_GUN_STATS", menuName = "UDEV/" + GameConsts.PROJECT_NAME + "/Create Gun Stats")]
    public class GunStats : Stats
    {
        [Header("Base Stats:")]
        public Stat damage;
        public Stat ammo;
        public Stat reloadTime;

        [Header("Upgrade Settings")]
        public int maxLevel;
        public int level;
        public int upPrice;
        public int upPriceUp;
        public float damageUp;
        public int ammoUp;
        public float reloadTimeUp;
        int m_levelInfo;

        public int AmmoUpInfo { get => ammoUp; }
        public float DamageUpInfo {
            get { m_levelInfo = level + 1; return damageUp * m_levelInfo; }
        }
        public float ReloadTimeUpInfo {
            get { m_levelInfo = level + 1; return reloadTimeUp * m_levelInfo; }
        }

        public override void SetStats()
        {
            m_stats = new Stat[] { damage, ammo, reloadTime };
        }

        public bool IsMaxLevel()
        {
            return level >= maxLevel;
        }

        public void Upgrade(string id)
        {
            if (!IsMaxLevel() && Prefs.IsEnoughCoin(upPrice))
            {
                Prefs.coins -= upPrice;

                level++;
                upPrice += upPriceUp * level;
                damage.baseValue += damageUp * level;
                ammo.baseValue += ammoUp;
                reloadTime.baseValue -= reloadTimeUp * level;
                reloadTime.baseValue = Mathf.Clamp(reloadTime.baseValue, 0, reloadTime.baseValue);

                SaveData(id);
            }
        }

        void SaveData(string id)
        {
            string json = JsonUtility.ToJson(this);

            CPlayerPrefs.SetString(GameConsts.GUN_DATA + id, json);
        }

        public void LoadData(string id)
        {
            string json = CPlayerPrefs.GetString(GameConsts.GUN_DATA + id, null);

            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }
    }
}
