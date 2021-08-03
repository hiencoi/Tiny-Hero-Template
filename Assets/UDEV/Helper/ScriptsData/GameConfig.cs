namespace UDEV
{
    using UnityEngine;
    using System;

    [System.Serializable]
    public class GameConfig
    {
        public Admob admob;

        [Header("Ads Settings:")]
        public int adPeriod;
        public int rewardedVideoPeriod;
        public int rewardedVideoAmount;
        [Header("Game Settings:")]
        public string androidPackageID;
        public string facebookPageID;
        public int startingCoins;
        public int dailyGiftCoins;
        public int coinsForRateGame;
    }

    [System.Serializable]
    public class Admob
    {
        [Header("App Id")]
        public string androidAppId;
        public string iosAppId;
        [Header("Interstitial")]
        public string androidInterstitial;
        public string iosInterstitial;
        [Header("Banner")]
        public string androidBanner;
        public string iosBanner;
        [Header("RewardedVideo")]
        public string androidRewarded;
        public string iosRewarded;
    }
}
