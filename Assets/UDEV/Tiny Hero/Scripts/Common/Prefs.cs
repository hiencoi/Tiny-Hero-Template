using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    public static class Prefs {

        public static bool firstTimeInGame
        {
            get
            {
                return CPlayerPrefs.GetBool(GameConsts.FIRST_TIME_IN_GAME, true);
            }
            set
            {
                CPlayerPrefs.SetBool(GameConsts.FIRST_TIME_IN_GAME, value);
            }
        }

        public static float musicVolume
        {
            get
            {
                return CPlayerPrefs.GetFloat(GameConsts.MUSIC_VOLUME, 0);
            }
            set
            {
                CPlayerPrefs.SetFloat(GameConsts.MUSIC_VOLUME, value);
            }
        }

        public static float soundVolume
        {
            get
            {
                return CPlayerPrefs.GetFloat(GameConsts.SOUND_VOLUME, 0);
            }
            set
            {
                CPlayerPrefs.SetFloat(GameConsts.SOUND_VOLUME, value);
            }
        }

        public static int coins {
            get {
                return CPlayerPrefs.GetInt(GameConsts.COIN, 0);
            }
            set {
                CPlayerPrefs.SetInt(GameConsts.COIN, value);
            }
        }

        public static bool UserRated {
            get
            {
                return CPlayerPrefs.GetBool(GameConsts.USER_RATED);
            }

            set
            {
                CPlayerPrefs.SetBool(GameConsts.USER_RATED, value);
            }
        }

        public static bool RateBtnClicked
        {
            get
            {
                return CPlayerPrefs.GetBool(GameConsts.RATE_BUTTON_CLICKED);
            }

            set
            {
                CPlayerPrefs.SetBool(GameConsts.RATE_BUTTON_CLICKED, value);
            }
        }

        public static void UnlockGun(string id, bool isUnlocked)
        {
            CPlayerPrefs.SetBool(GameConsts.GUN_UNLOCKED + id, isUnlocked);
        }

        public static bool IsGunUnlocked(string id)
        {
            return CPlayerPrefs.GetBool(GameConsts.GUN_UNLOCKED + id);
        }

        public static string currentGun
        {
            get
            {
                return CPlayerPrefs.GetString(GameConsts.CUR_GUN);
            }

            set
            {
                CPlayerPrefs.SetString(GameConsts.CUR_GUN, value);
            }
        }

        public static bool IsEnoughCoin(int price)
        {
            return coins >= price;
        }

        public static string currentMission
        {
            get
            {
                return CPlayerPrefs.GetString(GameConsts.CUR_MISSION);
            }

            set
            {
                CPlayerPrefs.SetString(GameConsts.CUR_MISSION, value);
            }
        }

        public static void UnlockMission(string id ,bool isUnlocked)
        {
            CPlayerPrefs.SetBool(GameConsts.MISSION_UNLOCKED + id, isUnlocked);
        }

        public static bool IsMissionUnlocked(string id)
        {
            return CPlayerPrefs.GetBool(GameConsts.MISSION_UNLOCKED + id, false);
        }
    }
}
