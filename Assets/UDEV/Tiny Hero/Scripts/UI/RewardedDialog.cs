using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class RewardedDialog : Dialog
    {
        public Text coinsBonusText;

        public override void Show()
        {
            base.Show();

            AudioController.Ins.EnableAudio(true);
            AudioController.Ins.PlaySound(AudioController.Ins.popupOrUpgrade);
            Time.timeScale = 0f;

            Timer.Schedule(this, 2f, () =>
            {
                Time.timeScale = 1f;
                Close();
            }, true);
        }

        public void UpdateDialogData(int coinsBonus)
        {
            Prefs.coins += coinsBonus;

            if (coinsBonusText)
                coinsBonusText.text = coinsBonus + "$";
        }

        public override void OnDialogCompleteClosed()
        {
            base.OnDialogCompleteClosed();
        }
    }
}
