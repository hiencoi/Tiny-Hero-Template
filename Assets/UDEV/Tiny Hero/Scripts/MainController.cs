using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    public class MainController : Singleton<MainController>
    {
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            if (Prefs.firstTimeInGame)
            {
                Prefs.musicVolume = AudioController.Ins.musicVolume;
                Prefs.soundVolume = AudioController.Ins.sfxVolume;

                Prefs.coins += ConfigController.Ins.config.startingCoins;

                Prefs.firstTimeInGame = false;
            }
            else
            {
                AudioController.Ins.musicVolume = Prefs.musicVolume;
                AudioController.Ins.sfxVolume = Prefs.soundVolume;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
