using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero {
    public class GunInfoDialog : Dialog
    {
        public Image hudIcon;
        public Text levelText;
        public Text ammoBaseText;
        public Text damageBaseText;
        public Text reloadBaseText;
        public Text ammoUpText;
        public Text damageUpText;
        public Text reloadUpText;
        public GameObject upgradeValues;
        public GameObject baseValues;
        public Button upBtn;
        public Image upBtnImage;
        public Text upBtnText;
        public Text cancelBtnText;

        public override void Close()
        {
            base.Close();

            Time.timeScale = 1f;

            var currentDialog = (GunShopDialog)DialogController.Ins.current;

            currentDialog.UpdateAll();
        }

        public override void OnDialogCompleteClosed()
        {
            Time.timeScale = 0f;
            base.OnDialogCompleteClosed();
        }


        public void UpdateInfo(GunController gun )
        {
            if (gun)
            {
                if (hudIcon)
                    hudIcon.sprite = gun.hudIcon;

                if (levelText)
                    levelText.text = gun.stats.level.ToString();

                if (ammoBaseText)
                    ammoBaseText.text = "AMMO : " + gun.stats.ammo.GetIntBaseValue().ToString();

                if (damageBaseText)
                    damageBaseText.text = "DAMAGE : " + gun.stats.damage.baseValue.ToString("F1");

                if (reloadBaseText)
                    reloadBaseText.text = "RELOAD : " + gun.stats.reloadTime.baseValue.ToString("F1") + "s";

                if (upBtnText)
                    upBtnText.text = "UP [" + gun.stats.upPrice.ToString() + "$]";

                if (Prefs.IsEnoughCoin(gun.stats.upPrice)
                    && !gun.stats.IsMaxLevel())
                {
                    if (ammoUpText)
                        ammoUpText.text = "+" + gun.stats.AmmoUpInfo.ToString();

                    if (damageUpText)
                        damageUpText.text = "+" + gun.stats.DamageUpInfo.ToString();

                    if (reloadUpText)
                        reloadUpText.text = "-" + gun.stats.ReloadTimeUpInfo.ToString() + "s";

                    if (cancelBtnText)
                        cancelBtnText.text = "CANCEL";

                    if (upBtn)
                    {
                        upBtn.onClick.RemoveAllListeners();
                        upBtn.onClick.AddListener(() => UpgradeEvent(gun));

                        upBtn.enabled = true; 
                    }
                }
                else
                {
                    if (cancelBtnText)
                        cancelBtnText.text = "CLOSE";

                    if (upgradeValues)
                        upgradeValues.SetActive(false);

                    if(upBtn)
                        upBtn.enabled = false;

                    if (upBtnImage)
                        upBtnImage.color = Color.gray;

                    if (gun.stats.IsMaxLevel())
                    {
                        if (upBtnText)
                            upBtnText.text = "[ MAX ]";
                    }
                }
            }
        }

        void UpgradeEvent(GunController gun)
        {
            if (gun)
            {
                gun.Upgrade();
                UpdateInfo(gun);
                AudioController.Ins.PlaySound(AudioController.Ins.popupOrUpgrade);
                GameUIManager.Ins.UpdateCoinsInfo();
            }
        }
    }
}
