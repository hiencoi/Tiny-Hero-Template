using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace UDEV.TinyHero
{
    public class CamController : MonoBehaviour
    {
        float m_shakeAmplitude = 0f;         // Cinemachine Noise Profile Parameter
        float m_shakeFrequency = 0f;         // Cinemachine Noise Profile Parameter
        float m_shakeElapsedTime = 0f;

        bool m_isShaking;
        float m_mapLimitX;

        // Cinemachine Shake
        public CinemachineVirtualCamera virtualCamera;
        public CinemachineConfiner confiner;
        private CinemachineBasicMultiChannelPerlin m_virtualCameraNoise;

        public bool IsShaking { get => m_isShaking; set => m_isShaking = value; }
        public float MapLimitX { get => m_mapLimitX;}

        private void Awake()
        {
            // Get Virtual Camera Noise Profile
            if (virtualCamera != null)
            {
                m_virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
                m_virtualCameraNoise.m_AmplitudeGain = 0;
                m_virtualCameraNoise.m_FrequencyGain = 0;
            }

            if (confiner && confiner.m_BoundingShape2D)
            {
                if (virtualCamera)
                {
                    float camWith = virtualCamera.m_Lens.OrthographicSize * virtualCamera.m_Lens.Aspect;
                    m_mapLimitX = confiner.m_BoundingShape2D.bounds.size.x / 2 + camWith + camWith / 2;
                }
            }
        }

        public void SetTarget(Transform target)
        {
            if (virtualCamera)
                virtualCamera.Follow = target;
        }

        // Update is called once per frame
        void Update()
        {
            if(m_isShaking)
                ShakeListener();
        }

        void ShakeListener()
        {
            // If the Cinemachine componet is not set, avoid update
            if (virtualCamera != null && m_virtualCameraNoise != null)
            {
                // If Camera Shake effect is still playing
                if (m_shakeElapsedTime > 0)
                {
                    // Set Cinemachine Camera Noise parameters
                    m_virtualCameraNoise.m_AmplitudeGain = m_shakeAmplitude;
                    m_virtualCameraNoise.m_FrequencyGain = m_shakeFrequency;

                    // Update Shake Timer
                    m_shakeElapsedTime -= Time.deltaTime;
                }
                else
                {
                    // If Camera Shake effect is over, reset variables
                    m_virtualCameraNoise.m_AmplitudeGain = 0f;
                    m_shakeElapsedTime = 0f;
                    m_isShaking = false;
                }
            }
        }

        public void ShakeTrigger(float dur, float amp, float freq, bool isOverride = false)
        {
            if (isOverride)
            {
                IsShaking = false;
            }

            if (!m_isShaking)
            {
                m_shakeElapsedTime = dur;
                m_shakeAmplitude = amp;
                m_shakeFrequency = freq;

                m_isShaking = true;
            }
        }
    }
}
