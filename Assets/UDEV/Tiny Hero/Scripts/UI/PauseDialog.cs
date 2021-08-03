using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class PauseDialog : Dialog
    {
        public Slider musicVolume;
        public Slider soundVolume;

        public override void Show()
        {
            base.Show();

            Time.timeScale = 0f;

            musicVolume.value = AudioController.Ins.musicVolume;

            soundVolume.value = AudioController.Ins.sfxVolume;
        }

        public override void Close()
        {
            base.Close();

            Time.timeScale = 1f;
        }

        public void OnMusicVolumeChange()
        {
            AudioController.Ins.musicVolume = musicVolume.value;

            Prefs.musicVolume = musicVolume.value;
        }

        public void OnSoundVolumeChange()
        {
            AudioController.Ins.sfxVolume = soundVolume.value;

            Prefs.soundVolume = soundVolume.value;
        }
    }
}
