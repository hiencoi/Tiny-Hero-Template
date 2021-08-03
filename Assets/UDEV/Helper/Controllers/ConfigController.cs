using UnityEngine;
using System.Collections;

namespace UDEV
{
    public class ConfigController : Singleton<ConfigController>
    {
        public GameConfig config;

        public string KeyPref
        {
            get { return "game_config"; }
        }

        public override void Awake()
        {
            base.Awake();
            GetConfig();
        }

        private void GetConfig()
        {
            if (CPlayerPrefs.HasKey(KeyPref))
            {
                try
                {
                    CPlayerPrefs.useRijndael(false);
                    string data = CPlayerPrefs.GetString(KeyPref);
                    CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);

                    var savedConfig = JsonUtility.FromJson<GameConfig>(data);
                    if (savedConfig != null) config = savedConfig;
                }
                catch { }
            }
        }

        public void ApplyConfig(string data)
        {
            CPlayerPrefs.useRijndael(false);
            CPlayerPrefs.SetString(KeyPref, data);
            CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);

            GetConfig();
        }
    }
}
