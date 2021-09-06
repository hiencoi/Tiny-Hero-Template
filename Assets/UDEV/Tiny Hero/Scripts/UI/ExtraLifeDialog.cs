using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class ExtraLifeDialog : Dialog
    {
        public override void Show()
        {
            base.Show();

            Time.timeScale = 0f;
        }

        public override void Close()
        {
            Time.timeScale = 1f;
            base.Close();
        }

        public void GetExtraLife()
        {
            AdmobController.Ins.rewardedCallback.rewardType = RewardType.LIFE;
            AdmobController.Ins.ShowRewardedVideo();
        }
    }
}
