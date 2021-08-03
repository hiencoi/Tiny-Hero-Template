using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class DailyGiftButton : RepeatingAction
    {
        public Animator anim;
        public GameObject timeCountingPanel;
        public TimerText timeCountingText;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();

            if (timeCountingText != null) timeCountingText.onCountDownComplete += OnCountDownComplete;
        }

        public override void UpdateAction()
        {
            if (!CUtils.IsActionAvailable(actionName, repeatRateSeconds))
            {
                if (timeCountingPanel)
                    timeCountingPanel.SetActive(true);

                if (timeCountingText)
                {
                    timeCountingText.SetTime((int)(repeatRateSeconds - CUtils.GetActionDeltaTime(actionName)));
                    timeCountingText.Run();
                }

                if (anim)
                    anim.enabled = false;
            }
            else
            {
                if (timeCountingPanel)
                    timeCountingPanel.SetActive(false);

                if (anim)
                    anim.enabled = true;
            }
        }

        public void OpenGift()
        {
            if (CUtils.IsActionAvailable(actionName, repeatRateSeconds))
            {
                DialogController.Ins.ShowDialog(DialogType.Rewarded, DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
                RewardedDialog rewardedDialog = (RewardedDialog)DialogController.Ins.current;
                rewardedDialog.UpdateDialogData(ConfigController.Ins.config.dailyGiftCoins);
                CUtils.SetActionTime(actionName);
                UpdateAction();
            }
        }

        void OnCountDownComplete()
        {
            if (timeCountingPanel)
                timeCountingPanel.SetActive(false);

            if (anim)
                anim.enabled = true;
        }
    }
}