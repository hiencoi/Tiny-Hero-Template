using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {

    [RequireComponent(typeof(GunController))]
    public class GunVisualHandle : MonoBehaviour
    {
        [Header("Camera Shake Visual Settings:")]
        public float shakeDuration = 0f;          // Time the Camera Shake effect will last
        public float shakeAmplitude = 0f;         // Cinemachine Noise Profile Parameter
        public float shakeFrequency = 0f;

        [Header("Sounds Settings:")]
        public AudioClip[] shootSounds;
        public AudioClip[] drySounds;
        public AudioClip[] reloadSounds;

        GunController m_gun;
        CamController m_cam;
        bool m_isDrySoundDone;

        private void Awake()
        {
            m_gun = GetComponent<GunController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            m_cam = GameManager.Ins.cam;

            if (m_gun)
            {
                m_gun.InShooting.AddListener(() => InShootingEvent());
                m_gun.InReloading.AddListener(() => InReloadingEvent());
                m_gun.ReloadFinish.AddListener(() => ReloadFinishEvent());
            }
        }

        void CamShake()
        {
            if (m_cam)
                m_cam.ShakeTrigger(shakeDuration, shakeAmplitude, shakeFrequency);
        }

        public void InShootingEvent()
        {
            CamShake();
            GameUIManager.Ins.UpdateBulletsInfo(m_gun.CurAmmo, m_gun.stats.ammo.GetIntValue());
            if (m_gun)
                AudioController.Ins.PlaySound(shootSounds);
        }

        public void ReloadFinishEvent()
        {
            GameUIManager.Ins.UpdateBulletsInfo(m_gun.CurAmmo, m_gun.stats.ammo.GetIntValue());
            if (m_gun)
                AudioController.Ins.PlaySound(reloadSounds);
        }

        public void PlayEquippedSounds()
        {
            AudioController.Ins.PlaySound(reloadSounds);
        }

        public void InReloadingEvent()
        {
            PlayDrySounds();
        }

        void PlayDrySounds()
        {
            if (m_gun && m_gun.CanShoot)
            {
                if (!m_isDrySoundDone)
                {
                    AudioController.Ins.PlaySound(drySounds);
                    m_isDrySoundDone = true;
                    StartCoroutine(PlayDrySoundDelay());
                }
            }
        }

        IEnumerator PlayDrySoundDelay()
        {
            yield return new WaitForSeconds(0.1f);

            m_isDrySoundDone = false;
        }
    }
}
