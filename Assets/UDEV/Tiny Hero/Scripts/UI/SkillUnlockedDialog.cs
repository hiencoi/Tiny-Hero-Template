using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class SkillUnlockedDialog : Dialog
    {
        public Image skillIcon;
        public Text skillNameText;

        public override void Show()
        {
            base.Show();

            Time.timeScale = 0f;

            AudioController.Ins.PlaySound(AudioController.Ins.popupOrUpgrade);

            Timer.Schedule(this, 2.5f, () =>
            {
                Time.timeScale = 1f;
                Close();
            }, true);
        }

        public void UpdateData(Skill skill)
        {
            if (skill)
            {
                if (skillIcon)
                    skillIcon.sprite = skill.hudIcon;

                if (skillNameText)
                    skillNameText.text = skill.name.ToUpper();
            }
        }

        public override void OnDialogCompleteClosed()
        {
            base.OnDialogCompleteClosed();
        }
    }
}
