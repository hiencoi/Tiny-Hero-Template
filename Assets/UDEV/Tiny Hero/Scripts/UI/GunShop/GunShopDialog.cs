using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.GridSlider;
using System.Linq;

namespace UDEV.TinyHero
{
    public class GunShopDialog : GridSliderDialog
    {
        GunShopData.ShopData[] m_data;
        Player m_player;

        public override void Show()
        {
            base.Show();

            m_data = GunShopData.Ins.shopDatas;
            m_data = m_data.OrderBy(p => p.price).ToArray();
            m_player = GameManager.Ins.Player;
            DrawSlider(m_data.Length);
            Time.timeScale = 0f;
        }

        public override void Close()
        {
            base.Close();
            Time.timeScale = 1f;
        }

        public override void ItemHandle(GameObject gridItem, int index)
        {
            GunShopItem itemUI = gridItem.GetComponent<GunShopItem>();

            if(m_data != null && m_data.Length > 0)
            {
                if (m_data[index] != null && itemUI)
                {
                    GunShopData.ShopData item = m_data[index];

                    if (Prefs.IsGunUnlocked(item.gun.id))
                    {
                        if (string.Compare(item.gun.id, Prefs.currentGun) == 0)
                        {
                            ItemSelectedUI(itemUI, item);
                        }
                        else
                        {
                            ItemUnlockedUI(itemUI, item);
                        }
                    }
                    else
                    {
                        if (Prefs.IsEnoughCoin(item.price))
                        {
                            ItemCanBuyUI(itemUI, item);
                        }
                        else
                        {
                            ItemLockedUI(itemUI, item);
                        }

                    } 

                    if (itemUI.buyBtn)
                    {
                        itemUI.buyBtn.onClick.RemoveAllListeners();
                        itemUI.buyBtn.onClick.AddListener(() => ItemEvent(item.gun, item.price));
                    }
                }
            }
        }

        void BaseItemUI(GunShopItem itemUI, GunShopData.ShopData item)
        {
            itemUI.UpdateIcon(item.gun.hudIcon);

            itemUI.UpdatePrice_State(item.price.ToString() + "$");

            itemUI.infoBtn.onClick.RemoveAllListeners();
            itemUI.infoBtn.onClick.AddListener(() => InfoBtnEvent(item.gun));
        }

        void ItemLockedUI(GunShopItem itemUI, GunShopData.ShopData item)
        {
            BaseItemUI(itemUI, item);

            itemUI.hudIcon.color = Color.black;

            itemUI.price_state_txt.color = Color.gray;

            itemUI.infoBtn.enabled = false;

            itemUI.buyBtn.enabled = false;

            itemUI.infoBtnImage.color = Color.gray;
        }

        void ItemCanBuyUI(GunShopItem itemUI, GunShopData.ShopData item)
        {
            BaseItemUI(itemUI, item);

            itemUI.hudIcon.color = Color.black;

            itemUI.price_state_txt.color = new Color32(221, 174, 0, 255);

            itemUI.buyBtn.enabled = true;

            itemUI.infoBtn.enabled = false;

            itemUI.infoBtnImage.color = Color.gray;
        }

        void ItemUnlockedUI(GunShopItem itemUI, GunShopData.ShopData item)
        {
            BaseItemUI(itemUI, item);

            itemUI.hudIcon.color = Color.white;

            itemUI.price_state_txt.color = new Color32(0, 163, 239, 255);

            itemUI.UpdatePrice_State("ready");

            itemUI.buyBtn.enabled = true;

            itemUI.infoBtn.enabled = true;

            itemUI.infoBtnImage.color = new Color32(73, 152, 0, 255);
        }

        void ItemSelectedUI(GunShopItem itemUI, GunShopData.ShopData item)
        {
            BaseItemUI(itemUI, item);

            itemUI.hudIcon.color = Color.white;

            itemUI.price_state_txt.color = new Color32(0, 163, 239, 255);

            itemUI.UpdatePrice_State("equipped");

            itemUI.buyBtn.enabled = true;

            itemUI.infoBtn.enabled = true;

            itemUI.infoBtnImage.color = new Color32(73, 152, 0, 255);
        }

        void ItemEvent(GunController gun, int price)
        {
            if (Prefs.IsGunUnlocked(gun.id))
            {
                if (string.Compare(gun.id, Prefs.currentGun) == 0) return;

                Prefs.currentGun = gun.id;
                UpdateAll();

                m_player.UpdateGun(GunShopData.Ins.curGun);
                GameUIManager.Ins.UpdateGunHud(gun.hudIcon);
                GameUIManager.Ins.UpdateBulletsInfo(m_player.Gun.CurAmmo, m_player.Gun.stats.ammo.GetIntValue());
            }
            else if(Prefs.IsEnoughCoin(price))
            {
                Prefs.coins -= price;
                Prefs.currentGun = gun.id;
                Prefs.UnlockGun(gun.id ,true);
                UpdateAll();

                m_player.UpdateGun(GunShopData.Ins.curGun);
                GameUIManager.Ins.UpdateGunHud(gun.hudIcon);
                GameUIManager.Ins.UpdateBulletsInfo(m_player.Gun.CurAmmo, m_player.Gun.stats.ammo.GetIntValue());
                GameUIManager.Ins.UpdateCoinsInfo();
            }
        }

        void InfoBtnEvent(GunController gun)
        {
            DialogController.Ins.ShowDialog(DialogType.GunInfo, DialogShow.OVER_CURRENT);

            var currentDialog = (GunInfoDialog)DialogController.Ins.current;

            currentDialog.UpdateInfo(gun);

            Hide();
        }
    }
}
