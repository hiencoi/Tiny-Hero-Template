using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV {
    public class RewardedButton : MonoBehaviour
    {
        public GameObject content;
        public GameObject adAvailableTextHolder;
        public TimerText timerText;

        private const string ACTION_NAME = "rewarded_video";
        private bool isEventAttached;

        private void Start()
        {
            if (timerText != null) timerText.onCountDownComplete += OnCountDownComplete;

#if UNITY_ANDROID || UNITY_IOS
        Timer.Schedule(this, 0.1f, AddEvents);

        if (!IsAvailableToShow())
        {
            content.SetActive(false);
            if (IsAdAvailable() && !IsActionAvailable())
            {
                int remainTime = (int)(ConfigController.Ins.config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }

        InvokeRepeating("IUpdate", 1, 1);
#else
            content.SetActive(false);
#endif
        }

        private void AddEvents()
        {
            if (AdmobController.Ins.rewardBasedVideo != null)
            {
                AdmobController.Ins.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            }
        }

        private void IUpdate()
        {
            if (content)
                content.SetActive(IsAvailableToShow());
        }

        public void OnClick()
        {
            AdmobController.Ins.ShowRewardBasedVideo();
        }

        private void ShowTimerText(int time)
        {
            if (adAvailableTextHolder != null)
            {
                adAvailableTextHolder.SetActive(true);
                timerText.SetTime(time);
                timerText.Run();
            }
        }

        public void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            content.SetActive(false);
            ShowTimerText(ConfigController.Ins.config.rewardedVideoPeriod);
        }

        private void OnCountDownComplete()
        {
            adAvailableTextHolder.SetActive(false);
            if (IsAdAvailable())
            {
                content.SetActive(true);
            }
        }

        public bool IsAvailableToShow()
        {
            return IsActionAvailable() && IsAdAvailable();
        }

        private bool IsActionAvailable()
        {
            return CUtils.IsActionAvailable(ACTION_NAME, ConfigController.Ins.config.rewardedVideoPeriod);
        }

        private bool IsAdAvailable()
        {
            if (AdmobController.Ins.rewardBasedVideo == null) return false;
            bool isLoaded = AdmobController.Ins.rewardBasedVideo.IsLoaded();
            return isLoaded;
        }

        private void OnDestroy()
        {
#if UNITY_ANDROID || UNITY_IOS
        if (AdmobController.Ins.rewardBasedVideo != null)
        {
            AdmobController.Ins.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        }
#endif
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (adAvailableTextHolder.activeSelf)
                {
                    int remainTime = (int)(ConfigController.Ins.config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                    ShowTimerText(remainTime);
                }
            }
        }
    }
}

