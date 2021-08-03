using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UDEV.TinyHero {
    public class GunShopData : Singleton<GunShopData>
    {
        [System.Serializable]
        public class ShopData {
            public GunController gun;
            public int price;
        }

        public GunController startingGun;

        public ShopData[] shopDatas;

        GunController[] m_gunsCreated;

        Player m_player;

        public GunController curGun
        {
            get
            {
                if(IsGunsCreatedEmpty())
                {
                    InstantiateGuns();
                }

                if (!CPlayerPrefs.HasKey(GameConsts.CUR_GUN))
                {
                    for (int i = 0; i < m_gunsCreated.Length; i++)
                    {
                        if (string.Compare(m_gunsCreated[i].id, startingGun.id) == 0)
                        {
                            Prefs.UnlockGun(m_gunsCreated[i].id, true);
                            Prefs.currentGun = m_gunsCreated[i].id;
                            return m_gunsCreated[i];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < m_gunsCreated.Length; i++)
                    {
                        if (string.Compare(m_gunsCreated[i].id, Prefs.currentGun) == 0)
                            return m_gunsCreated[i];
                    }
                }

                return null;
            }
        }

        public GunController[] GunCreated { get => m_gunsCreated; set => m_gunsCreated = value; }
        public Player Player { get => m_player; set => m_player = value; }

        void InstantiateGuns()
        {
            if (shopDatas != null && shopDatas.Length >= 0)
            {
                m_gunsCreated = new GunController[shopDatas.Length];

                for (int i = 0; i < shopDatas.Length; i++)
                {
                    if (shopDatas[i].gun)
                    {
                        m_gunsCreated[i] = Instantiate(shopDatas[i].gun, Vector3.zero, Quaternion.identity);
                        m_gunsCreated[i].gameObject.SetActive(false);
                        m_gunsCreated[i].transform.SetParent(m_player.transform);
                        m_gunsCreated[i].transform.localScale = shopDatas[i].gun.transform.localScale;
                        m_gunsCreated[i].transform.localPosition = m_player.gunPoint.localPosition + (Vector3)m_player.gunPointOffset;
                        m_gunsCreated[i].Player = m_player;
                    }
                }
            }
        }

        bool IsGunsCreatedEmpty()
        {
            if (m_gunsCreated == null || m_gunsCreated.Length <= 0) return true;
            else
            {
                for (int i = 0; i < m_gunsCreated.Length; i++)
                {
                    if(m_gunsCreated[i] == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
