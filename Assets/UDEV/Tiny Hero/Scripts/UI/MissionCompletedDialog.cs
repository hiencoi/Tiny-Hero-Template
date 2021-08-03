using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class MissionCompletedDialog : Dialog
    {
        public Text enemiesKilledTxt;
        public Text coinsCollectedText;
        public Text coinsBonusTxt;
        public Text xpBonusTxt;
        public Text scoreTxt;

        public override void Show()
        {
            base.Show();

            GameUIManager.Ins.ShowGameGui(false);

            UpdateInfo();
        }

        public override void Close()
        {
            base.Close();

            Time.timeScale = 1f;
        }

        void UpdateInfo()
        {
            int enemiesKilled = GameManager.Ins.EnemiesKilled;

            int coinsCollected = GameManager.Ins.CoinsCollected;

            int coinsBonus = GameManager.Ins.CoinsBonus;

            int xpBonus = GameManager.Ins.XpBonus;

            int score = GameManager.Ins.Score;

            if (enemiesKilledTxt)
                enemiesKilledTxt.text = enemiesKilled.ToString();

            if (coinsCollectedText)
                coinsCollectedText.text = coinsCollected.ToString();

            if (coinsBonusTxt)
                coinsBonusTxt.text = coinsBonus.ToString();

            if (xpBonusTxt)
                xpBonusTxt.text = xpBonus.ToString();

            if (scoreTxt)
                scoreTxt.text = score.ToString();
        }
    }
}
