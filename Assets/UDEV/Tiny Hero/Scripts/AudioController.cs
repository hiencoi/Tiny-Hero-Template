using UnityEngine;
using UDEV.TinyHero;

namespace UDEV
{
    public class AudioController : Singleton<AudioController>
    {

        [Header("Main Settings:")]
        [Range(0, 1)]
        public float musicVolume = 0.3f;
        /// the sound fx volume
        [Range(0, 1)]
        public float sfxVolume = 1f;

        public AudioSource musicAus;
        public AudioSource sfxAus;

        [Header("Musics And Sounds: ")]
        public AudioClip[] victorySound;
        public AudioClip[] backgroundMusics;
        public AudioClip[] popupOrUpgrade;

        /// <summary>
        /// Play Sound Effect
        /// </summary>
        /// <param name="clips">Array of sounds</param>
        /// <param name="aus">Audio Source</param>
        public void PlaySound(AudioClip[] clips, AudioSource aus = null)
        {
            if (!aus)
            {
                aus = sfxAus;
            }

            if (clips != null && clips.Length > 0 && aus)
            {
                var randomIdx = Random.Range(0, clips.Length);
                aus.PlayOneShot(clips[randomIdx], sfxVolume);
            }
        }

        /// <summary>
        /// Play Sound Effect
        /// </summary>
        /// <param name="clip">Sounds</param>
        /// <param name="aus">Audio Source</param>
        public void PlaySound(AudioClip clip, AudioSource aus = null)
        {
            if (!aus)
            {
                aus = sfxAus;
            }

            if (clip != null && aus)
            {
                aus.PlayOneShot(clip, sfxVolume);
            }
        }

        /// <summary>
        /// Play Music
        /// </summary>
        /// <param name="musics">Array of musics</param>
        /// <param name="loop">Can Loop</param>
        public void PlayMusic(AudioClip[] musics, bool loop = true)
        {
            if (musicAus && musics != null && musics.Length > 0)
            {
                var randomIdx = Random.Range(0, musics.Length);

                musicAus.clip = musics[randomIdx];
                musicAus.loop = loop;
                musicAus.volume = musicVolume;
                musicAus.Play();
            }
        }

        /// <summary>
        /// Play Music
        /// </summary>
        /// <param name="music">music</param>
        /// <param name="canLoop">Can Loop</param>
        public void PlayMusic(AudioClip music, bool canLoop)
        {
            if (musicAus && music != null)
            {
                musicAus.clip = music;
                musicAus.loop = canLoop;
                musicAus.volume = musicVolume;
                musicAus.Play();
            }
        }

        /// <summary>
        /// Set volume for audiosource
        /// </summary>
        /// <param name="vol">New Volume</param>
        public void SetMusicVolume(float vol)
        {
            if (musicAus) musicAus.volume = vol;
        }

        public void EnableAudio(bool isOn)
        {
            if (musicAus == null || sfxAus == null) return;

            musicAus.volume = isOn ? musicVolume : 0f;
            sfxAus.volume = isOn ? sfxVolume : 0f;
            if(!isOn)
                musicAus.Stop();
        }

        /// <summary>
        /// Stop play music or sound effect
        /// </summary>
        public void StopPlayMusic()
        {
            if (musicAus) musicAus.Stop();
        }
    }
}
