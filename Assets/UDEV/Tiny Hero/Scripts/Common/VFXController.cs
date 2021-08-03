using UnityEngine;

namespace UDEV.TinyHero
{
    public class VFXController : MonoBehaviour
    {
        public bool useCameraShake;
        public float shakeDuration = 0f;          // Time the Camera Shake effect will last
        public float shakeAmplitude = 0f;         // Cinemachine Noise Profile Parameter
        public float shakeFrequency = 0f;
        AudioSource aus;
        public AudioClip[] sounds;

        CamController m_cam;

        private void Awake()
        {
            aus = GetComponent<AudioSource>();
        }

        private void Start()
        {
            m_cam = GameManager.Ins.cam;
            CameraShakeVFX();
        }

        private void OnEnable()
        {
            if (AudioController.Ins) AudioController.Ins.PlaySound(sounds, aus);

            CameraShakeVFX();
        }

        private void OnDisable()
        {
            if (aus) aus.Stop();
        }

        void CameraShakeVFX()
        {
            if (useCameraShake && m_cam)
            {
                m_cam.ShakeTrigger(shakeDuration, shakeAmplitude, shakeFrequency, true);
            }
        }
    }
}
