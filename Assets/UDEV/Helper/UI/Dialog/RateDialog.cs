using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UDEV.TinyHero;

namespace UDEV
{
    public class RateDialog : YesNoDialog
    {
        public Text coinsBonusText;
        int m_coinsBonus;

        public override void Show()
        {
            if (Prefs.UserRated) return;

            base.Show();

            AudioController.Ins.EnableAudio(true);
            AudioController.Ins.PlaySound(AudioController.Ins.popupOrUpgrade);

            m_coinsBonus = ConfigController.Ins.config.coinsForRateGame;

            if (coinsBonusText)
                coinsBonusText.text = m_coinsBonus + "$";
        }

        public override void OnYesClick()
        {
            base.OnYesClick();
            CUtils.RateGame();
            Prefs.RateBtnClicked = true;
        }

        public override void OnNoClick()
        {
            base.OnNoClick();
        }
    }
}
