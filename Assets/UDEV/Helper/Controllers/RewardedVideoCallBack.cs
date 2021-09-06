using GoogleMobileAds.Api;
using UnityEngine;
using UDEV.TinyHero;

namespace UDEV
{
    public enum RewardType
    {
        COIN,
        LIFE
    }

    public class RewardedVideoCallBack : MonoBehaviour
    {
        public RewardType rewardType;

        int m_coinsReward;

        public int CoinsReward { get => m_coinsReward; set => m_coinsReward = value; }

        private void Start()
        {
            Timer.Schedule(this, 0.1f, AddEvents);
        }

        private void AddEvents()
        {
#if UNITY_ANDROID || UNITY_IOS
            AdmobController admobController = AdmobController.Ins;
            admobController.OnUserEarnedRewardEvent.AddListener(() => AdmobHandleRewardBasedVideoRewarded(this, null));
#endif
        }

        private const string ACTION_NAME = "rewarded_video";
        public void AdmobHandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            switch (rewardType)
            {
                case RewardType.COIN:
                    DialogController.Ins.current.onDialogCompleteClosed += () =>
                    {
                        Timer.Schedule(this, 0.3f, () =>
                        {
                            DialogController.Ins.ShowDialog(DialogType.Rewarded, DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
                            RewardedDialog rewardedDialog = (RewardedDialog)DialogController.Ins.current;
                            rewardedDialog.UpdateDialogData(CoinsReward);
                            rewardedDialog.onDialogCompleteClosed += () =>
                            {
                                GameManager.Ins.CoinsCollected += CoinsReward;
                                GameUIManager.Ins.UpdateCoinsInfo();
                            };
                        });
                    };

                    DialogController.Ins.CloseCurrentDialog();
                    break;
                case RewardType.LIFE:
                    DialogController.Ins.CloseCurrentDialog();
                    GameManager.Ins.AddExtraLife();
                    break;

            }
            CUtils.SetActionTime(ACTION_NAME);
        }

        private void OnDestroy()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            AdmobController admobController = AdmobController.Ins;
            admobController.OnUserEarnedRewardEvent.RemoveListener(() => AdmobHandleRewardBasedVideoRewarded(this, null));
#endif
        }
    }
}
