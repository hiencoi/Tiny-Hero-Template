using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class GunShopItem : MonoBehaviour
    {
        public Button buyBtn;
        public Button infoBtn;
        public Image infoBtnImage;

        public Image hudIcon;

        public Text price_state_txt;

        public void UpdateIcon(Sprite _icon)
        {
            if (hudIcon)
                hudIcon.sprite = _icon;
        }

        public void UpdatePrice_State(string _price)
        {
            if (price_state_txt)
                price_state_txt.text = _price;
        }
    }
}
