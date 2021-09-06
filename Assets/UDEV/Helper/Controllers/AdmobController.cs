using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Events;

namespace UDEV
{
    public class AdmobController : Singleton<AdmobController>
    {
        public RewardedVideoCallBack rewardedCallback;
        public GameConfig config;
        public BannerView bannerView;
        public InterstitialAd interstitialAd;
        public RewardedAd rewardedAd;

        public UnityEvent OnAdLoadedEvent;
        public UnityEvent OnAdFailedToLoadEvent;
        public UnityEvent OnAdOpeningEvent;
        public UnityEvent OnAdFailedToShowEvent;
        public UnityEvent OnUserEarnedRewardEvent;
        public UnityEvent OnAdClosedEvent;
        public bool showFpsMeter = true;

        public override void Start()
        {
            base.Start();
            config = ConfigController.Ins.config;
            Init();
        }

        #region Admob
        public void Init()
        {
            MobileAds.SetiOSAppPauseOnBackground(true);

            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

            // Add some test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
            deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
#endif

            // Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                .SetTestDeviceIds(deviceIds).build();

            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);
            if (!CUtils.IsBuyItem() && !CUtils.IsAdsRemoved())
            {
                RequestBanner();
                RequestInterstitial();
                HideBanner();
            }

            RequestRewardedVideo();


        }

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {

            });
        }

        public void RequestBanner()
        {
            // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = config.androidBanner.Trim();
#elif UNITY_IPHONE
        string adUnitId = config.iosBanner.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

            // Clean up banner before reusing
            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            // Create a 320x50 banner at top of the screen
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

            // Add Event Handlers
            bannerView.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
            bannerView.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoadEvent.Invoke();
            bannerView.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
            bannerView.OnAdClosed += (sender, args) => OnAdClosedEvent.Invoke();

            // Load a banner ad
            bannerView.LoadAd(CreateAdRequest());
        }

        public void RequestInterstitial()
        {
            // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = config.androidInterstitial.Trim();
#elif UNITY_IPHONE
        string adUnitId = config.iosInterstitial.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

            // Clean up interstitial before using it
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }

            interstitialAd = new InterstitialAd(adUnitId);

            // Add Event Handlers
            interstitialAd.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
            interstitialAd.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoadEvent.Invoke();
            interstitialAd.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
            interstitialAd.OnAdClosed += (sender, args) => {
                OnAdClosedEvent.Invoke();

#if UNITY_EDITOR
                RequestInterstitial();
#elif UNITY_ANDROID
                if (string.Compare(adUnitId, config.androidInterstitial.Trim()) == 0)
                    RequestInterstitial();
#elif UNITY_IPHONE
                if (string.Compare(adUnitId, config.iosInterstitial.Trim()) == 0)
                    RequestInterstitial();
#else
                RequestInterstitial();
#endif
            };

            // Load an interstitial ad
            interstitialAd.LoadAd(CreateAdRequest());
        }

        public void RequestRewardedVideo()
        {
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = config.androidRewarded.Trim();
#elif UNITY_IPHONE
        string adUnitId = config.iosRewarded.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif
            if (rewardedAd != null)
                rewardedAd.Destroy();

            rewardedAd = new RewardedAd(adUnitId);

            // Add Event Handlers
            rewardedAd.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
            rewardedAd.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoadEvent.Invoke();
            rewardedAd.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
            rewardedAd.OnAdFailedToShow += (sender, args) => OnAdFailedToShowEvent.Invoke();
            rewardedAd.OnAdClosed += (sender, args) => OnAdClosedEvent.Invoke();
            rewardedAd.OnUserEarnedReward += (sender, args) => OnUserEarnedRewardEvent.Invoke();
            OnAdClosedEvent.AddListener(() => {
#if UNITY_EDITOR
                RequestInterstitial();
                RequestRewardedVideo();
#elif UNITY_ANDROID
                if (string.Compare(adUnitId, config.androidRewarded.Trim()) == 0)
                    RequestRewardedVideo();
                else if (string.Compare(adUnitId, config.androidInterstitial.Trim()) == 0)
                    RequestInterstitial();
#elif UNITY_IPHONE
                if (string.Compare(adUnitId, config.iosRewarded.Trim()) == 0)
                    RequestRewardedVideo();
                else if (string.Compare(adUnitId, config.iosInterstitial.Trim()) == 0)
                    RequestInterstitial();
#else
                RequestRewardedVideo();
#endif
            });

            // Create empty ad request
            rewardedAd.LoadAd(CreateAdRequest());

        }

        // Returns an ad request with custom ad targeting.
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
                    .Build();
        }

        public void ShowInterstitial()
        {
            if (CUtils.IsBuyItem()) return;

            if (interstitialAd != null && interstitialAd.IsLoaded())
            {
                interstitialAd.Show();
            }
        }

        public void ShowBanner(int pos = (int)AdPosition.Bottom)
        {
            if (CUtils.IsBuyItem() && CUtils.IsAdsRemoved()) return;
            if (bannerView != null)
            {
                bannerView.SetPosition((AdPosition)pos);
                bannerView.Show();
            }
        }

        public void HideBanner()
        {
            if (bannerView != null)
            {
                bannerView.Hide();
            }
        }

        public bool ShowInterstitial(bool video = false)
        {
            if (CUtils.IsBuyItem()) return false;
            if (interstitialAd != null && interstitialAd.IsLoaded())
            {
                interstitialAd.Show();
                return true;
            }
            return false;
        }

        public void ShowRewardedVideo()
        {
            if (rewardedAd != null)
            {
                rewardedAd.Show();
            }
            else
            {
                RequestRewardedVideo();
            }
        }

        public bool IsRewardedVideoAvaiable()
        {
            if (rewardedAd == null) return false;
            bool isLoaded = rewardedAd.IsLoaded();
            return isLoaded;
        }

        #endregion;
    }
}
