using UnityEngine;
using System;

namespace UDEV
{
    public class JobWorker : Singleton<JobWorker>
    {
        public Action<string> onEnterScene;
        public Action onLink2Store;
        public Action onDailyGiftReceived;
        public Action onShowBanner;
        public Action onCloseBanner;
        public Action onShowFixedBanner;
        public Action onShowInterstitial;
    }
}