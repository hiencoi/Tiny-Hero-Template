using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.AI2D;
using UDEV.SPM;

namespace UDEV.TinyHero
{
    [RequireComponent(typeof(Player))]
    public class PlayerVisualHandle : MonoBehaviour
    {
        [Header("UI:")]
        [PoolerKeys(target = PoolerTarget.PLAYER)]
        public string healthBar;
        public float hpBarYOffset;
        public Vector3 hbBarScale = Vector3.one;

        [PoolerKeys(target = PoolerTarget.PLAYER)]
        public string reloadingBarPool;
        public float reloadingBarYOffset;
        public Vector3 reloadingBarBarScale = Vector3.one;

        [Header("VFX Effects")]
        [PoolerKeys(target = PoolerTarget.VFX)]
        public string landDustPool;
        public Transform landPoint;

        [PoolerKeys(target = PoolerTarget.VFX)]
        public string reviveVfxPool;
        [PoolerKeys(target = PoolerTarget.VFX)]
        public string levelUpVfxPool;
        [PoolerKeys(target = PoolerTarget.VFX)]
        public string deathVfxPool;

        public FlashVfx flashVfx;

        [Header("Sounds Effect")]
        public AudioClip[] jumpingSounds;
        public AudioClip[] landingSounds;
        public AudioClip[] lostLifeSounds;
        public AudioClip[] levelupSounds;
        public AudioClip[] deathSounds;

        Player m_player;

        ImageFilled m_healthBar; // Health bar UI
        ImageFilled m_reloadingBar;
        List<Skill> m_unlockedSkills;

        public ImageFilled ReloadingBar { get => m_reloadingBar;}

        private void Awake()
        {
            m_player = GetComponent<Player>();
        }

        private void Update()
        {
            if (m_healthBar)
            {
                m_healthBar.UpdateValue((float)m_player.CurHealth, (float)m_player.stats.health.GetValue());
            }

            if (m_reloadingBar)
            {
                m_reloadingBar.UpdateValue((float)m_player.Gun.CurReloadTime, (float)m_player.Gun.stats.reloadTime.GetValue(), true);
            }
        }

        void LateUpdate()
        {
            GameUIManager.Ins.UpdateHpInfo(Mathf.RoundToInt(m_player.CurHealth), m_player.stats.health.GetIntValue());
            GameUIManager.Ins.UpdateLevelFilledInfo(m_player.stats.xp, m_player.stats.levelUpXp);

            //Update position and vale of health bar UI
            if (m_healthBar)
            {
                m_healthBar.transform.position =
                    new Vector3(transform.position.x,
                    transform.position.y + hpBarYOffset,
                    transform.position.z);
            }

            //Update position and vale of gun reloading bar UI
            if (m_reloadingBar)
            {
                m_reloadingBar.transform.position =
                    new Vector3(transform.position.x,
                    transform.position.y + reloadingBarYOffset,
                    transform.position.z);
            }
        }

        public void OnInitEvent()
        {
            GameUIManager.Ins.UpdateHpInfo(Mathf.RoundToInt(m_player.CurHealth), m_player.stats.health.GetIntValue());
            GameUIManager.Ins.UpdateLevelFilledInfo((float)m_player.stats.xp, (float)m_player.stats.levelUpXp);
            GameUIManager.Ins.UpdateLevelInfoText(m_player.stats.level, m_player.stats.IsMaxLevel());
            GameUIManager.Ins.UpdateBulletsInfo(m_player.Gun.CurAmmo, m_player.Gun.stats.ammo.GetIntValue());
            GameUIManager.Ins.UpdateGunHud(m_player.Gun.hudIcon);

            //Spawn and update health bar UI
            GameObject hpBar = PoolersManager.Ins.Spawn(PoolerTarget.PLAYER, healthBar, transform.position, Quaternion.identity);

            if (hpBar)
            {
                hpBar.transform.localScale = hbBarScale;
                m_healthBar = hpBar.GetComponent<ImageFilled>();

                if (m_healthBar)
                {
                    m_healthBar.UpdateValue((float)m_player.CurHealth, (float)m_player.stats.health.GetValue());
                    m_healthBar.Root = transform;
                }
            }

            //Spawn and update reloading bar UI
            GameObject reloadingBar = PoolersManager.Ins.Spawn(PoolerTarget.PLAYER, reloadingBarPool, transform.position, Quaternion.identity);

            if (reloadingBar)
            {
                reloadingBar.transform.localScale = reloadingBarBarScale;
                m_reloadingBar = reloadingBar.GetComponent<ImageFilled>();

                if (m_reloadingBar)
                {
                    m_reloadingBar.UpdateValue((float)m_player.Gun.CurReloadTime, (float)m_player.Gun.stats.reloadTime.GetValue(), true);
                    m_reloadingBar.Root = transform;
                    m_reloadingBar.gameObject.SetActive(false);

                    OnUpdateGunEvent();
                }
            }

            Timer.Schedule(this, 0.3f, () =>
            {
                if (Prefs.RateBtnClicked && !Prefs.UserRated)
                {
                    DialogController.Ins.ShowDialog(DialogType.Rewarded, DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
                    RewardedDialog rewardedDialog = (RewardedDialog)DialogController.Ins.current;
                    rewardedDialog.UpdateDialogData(ConfigController.Ins.config.coinsForRateGame);
                    rewardedDialog.onDialogCompleteClosed += () =>
                    {
                        GameUIManager.Ins.UpdateCoinsInfo();
                        Prefs.UserRated = true;
                        DialogController.Ins.ShowDialog(DialogType.SkillsSelect, DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
                    };
                }
                else
                {
                    DialogController.Ins.ShowDialog(DialogType.SkillsSelect, DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
                }
            });
        }

        public void OnUpdateGunEvent()
        {
            if (m_reloadingBar)
            {
                m_player.Gun.InReloading.RemoveListener(() => m_reloadingBar.Show(true));
                m_player.Gun.ReloadFinish.RemoveListener(() => m_reloadingBar.Show(false));
                m_player.Gun.InReloading.AddListener(() => m_reloadingBar.Show(true));
                m_player.Gun.ReloadFinish.AddListener(() => m_reloadingBar.Show(false));
            }
        }

        public void OnDeadEvent()
        {
            if (m_healthBar) m_healthBar.Show(false);

            if (flashVfx)
            {
                flashVfx.StopFlash();
                flashVfx.SetSpritesAlpha(flashVfx.normalColor);
            }

            PoolersManager.Ins.Spawn(PoolerTarget.VFX, deathVfxPool, transform.position, Quaternion.identity);

            AudioController.Ins.PlaySound(deathSounds);
        }

        public void OnRiviveEvent()
        {
            if (m_healthBar) m_healthBar.Show(true);

            GameObject reviveVfx = PoolersManager.Ins.Spawn(PoolerTarget.VFX, reviveVfxPool, Vector3.zero, Quaternion.identity);

            if (reviveVfx)
            {
                reviveVfx.transform.SetParent(m_player.transform.GetChild(0));
                reviveVfx.transform.localScale = Vector3.one;
                reviveVfx.transform.localPosition = Vector3.zero;
            }
        }

        public void OnLandEvent()
        {
            if (landPoint)
                PoolersManager.Ins.Spawn(PoolerTarget.VFX, landDustPool, landPoint.position, Quaternion.identity);
            AudioController.Ins.PlaySound(landingSounds);
        }

        public void OnJumpEvent()
        {
            AudioController.Ins.PlaySound(jumpingSounds);
        }

        public void OnLostLifeEvent()
        {
            if(GameManager.Ins.CurLife > 0)
                AudioController.Ins.PlaySound(lostLifeSounds);
        }

        public void OnLevelupEvent()
        {
            GameUIManager.Ins.UpdateLevelFilledInfo((float)m_player.stats.xp, (float)m_player.stats.levelUpXp);
            GameUIManager.Ins.UpdateLevelInfoText(m_player.stats.level, m_player.stats.IsMaxLevel());
            AudioController.Ins.PlaySound(levelupSounds);

            GameObject levelUpVfx = PoolersManager.Ins.Spawn(PoolerTarget.VFX, levelUpVfxPool, Vector3.zero, Quaternion.identity);

            if (levelUpVfx)
            {
                levelUpVfx.transform.SetParent(m_player.transform.GetChild(0));
                levelUpVfx.transform.localScale = Vector3.one;
                levelUpVfx.transform.localPosition = Vector3.zero;
            }

            m_unlockedSkills = GameManager.Ins.Player.skillsManager.GetUlockedSkills(m_player);

            Timer.Schedule(this, 1f, () =>
            {
                ShowSkillUnlocked();
            });
        }

        void ShowSkillUnlocked()
        {
            if (m_unlockedSkills != null && m_unlockedSkills.Count > 0)
            {
                DialogController.Ins.ShowDialog(DialogType.SkillUnlocked, DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
                SkillUnlockedDialog skillUnlockedDialog = (SkillUnlockedDialog)DialogController.Ins.current;
                skillUnlockedDialog.UpdateData(m_unlockedSkills[0]);
                skillUnlockedDialog.onDialogCompleteClosed = () =>
                {
                    m_unlockedSkills.RemoveAt(0);

                    Timer.Schedule(this, 0.3f, () =>
                    {
                        ShowSkillUnlocked();
                    });
                };
            }
        }

        public void TakeDamageFlash()
        {
            GameUIManager.Ins.UpdateHpInfo(Mathf.RoundToInt(m_player.CurHealth), m_player.stats.health.GetIntValue());
            if (flashVfx)
                flashVfx.Flash(m_player.invincibleTime);
        }

        private void OnDrawGizmos()
        {
            if (!string.IsNullOrEmpty(healthBar))
            {
                Gizmos.DrawIcon(transform.position + new Vector3(0f, hpBarYOffset, 0f), "HPBar_Icon.png", true);
            }

            if (!string.IsNullOrEmpty(reloadingBarPool))
            {
                Gizmos.DrawIcon(transform.position + new Vector3(0f, reloadingBarYOffset, 0f), "GunReloadingBar_Icon.png", true);
            }
        }
    }
}
