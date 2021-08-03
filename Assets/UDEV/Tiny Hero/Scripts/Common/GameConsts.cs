using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    public static class GameConsts {
        #region Editor_Consts
        public const string PROJECT_NAME = "Tiny Hero";

        public const string EDITOR_DATA_PATH = "Assets/UDEV/"+ PROJECT_NAME +"/Scripts/Editor/Data/";

        public const string MISSION_SAVE_PATH = "Assets/UDEV/" + PROJECT_NAME + "/GameDatas/Missions/";
        #endregion

        #region Common_Game_Consts
        public const string COIN = "coin";

        public const string MISSION_UNLOCKED = "mission_unlocked_";

        public const string CUR_MISSION = "current_mission";

        public const string SKILL_DATA = "skill_data_";

        public const string PLAYER_DATA = "player_data_";

        public const string USER_RATED = "user_rated";

        public const string RATE_BUTTON_CLICKED = "rated_clicked";
        #endregion

        #region Game_Settings_Consts
        public const string FIRST_TIME_IN_GAME = "first_time_in_game";
        public const string MUSIC_VOLUME = "music_volume";
        public const string SOUND_VOLUME = "sound_volume";
        #endregion

        #region Gun_Consts
        public const string GUN_UNLOCKED = "gun_unlocked_";

        public const string CUR_GUN = "current_gun";

        public const string GUN_DATA = "gun_data_";
        #endregion
    }
}
