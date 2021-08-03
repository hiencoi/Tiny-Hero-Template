using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class SkillDialogItem : MonoBehaviour
    {
        public Image hudIcon;

        public Button selectBtn;

        public Toggle toggleBtn;

        public Button infoBtn;

        public Image infoBtnImage;

        private void Start()
        {
            if (toggleBtn)
            {
                toggleBtn.isOn = false;
            }
        }

        public void UpdateHudIcon(Sprite newIcon)
        {
            if (hudIcon)
                hudIcon.sprite = newIcon;
        }
    }
}
