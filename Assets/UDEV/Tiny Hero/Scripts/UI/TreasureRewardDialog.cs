using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class TreasureRewardDialog : Dialog
    {
        public Text coinsRewardText;
        private TreasureCollectable m_treasure;

        public TreasureCollectable Treasure { get => m_treasure; set => m_treasure = value; }

        public override void Show()
        {
            base.Show();

            Time.timeScale = 0f;
        }

        public void UpdateInfo()
        {
            if (m_treasure && coinsRewardText)
                coinsRewardText.text = "+" + m_treasure.CoinsReward;
        }

        public override void Close()
        {
            base.Close();

            Time.timeScale = 1f;
        }

        public void Open()
        {
            AdmobController.Ins.rewardedCallback.rewardType = RewardType.COIN;
            AdmobController.Ins.rewardedCallback.CoinsReward = m_treasure.CoinsReward;
            AdmobController.Ins.ShowRewardBasedVideo();
        }
    }
}
