using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UDEV.SPM;

namespace UDEV.TinyHero
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Game Settings: ")]
        public int maxLife;
        public int maxRewardLife;
        int m_curLife;
        int m_score;
        int m_enemiesKilled;
        int m_coinsCollected;
        int m_coinsBonus;
        int m_xpBonus;
        int m_lifeRewardUsed;

        public UnityEvent OnGameover;

        bool m_isGameover;
        bool m_isGamebegin;

        Player m_player;

        public GameObject mainCam;
        public CamController cam;

        public Player Player { get => m_player; set => m_player = value; }
        public int Score {
            get => m_score;
            set {
                m_score = value;
                GameUIManager.Ins.UpdateScoreInfo(value);
            }
        }
        public bool IsGamebegin { get => m_isGamebegin; set => m_isGamebegin = value; }
        public int EnemiesKilled { get => m_enemiesKilled; set => m_enemiesKilled = value; }
        public int CoinsCollected { get => m_coinsCollected; set => m_coinsCollected = value; }
        public int CurLife {
            get => m_curLife;
            set {
                m_curLife = value;
                GameUIManager.Ins.UpdateLife(m_curLife);
            } }
        public bool IsGameover { get => m_isGameover; private set => m_isGameover = value; }
        public int CoinsBonus { get => m_coinsBonus;}
        public int XpBonus { get => m_xpBonus;}

        public override void Awake()
        {
            MakeSingleton(false);

            PoolersManager.Ins.ClearAll();
        }

        public override void Start()
        {
            base.Start();

            Init();

            AudioController.Ins.EnableAudio(true);
        }

        void Init()
        {
            m_curLife = maxLife;

            MissionsManager.Ins.Init();

            OnGameover.AddListener(() => m_player.Die());

            OnGameover.AddListener(() => m_player.skillsManager.ResetSkills());

            MissionsManager.Ins.MissionController.AddEvent(
                    MissionController.SpawnStates.MissionComplete,
                    MissionComplete
                );

            if (cam)
                cam.virtualCamera.Follow = m_player.transform;

            GameUIManager.Ins.ShowGameGui(false);
            GameUIManager.Ins.UpdateLife(m_curLife);
            GameUIManager.Ins.UpdateCoinsInfo();
            GameUIManager.Ins.UpdateScoreInfo(m_score);
        }

        public void AddExtraLife()
        {
            m_lifeRewardUsed++;

            m_curLife++;

            m_player.Revive();

            GameUIManager.Ins.UpdateLife(m_curLife);

            m_isGameover = false;
        }

        /// <summary>
        /// Check player can get extra life or not
        /// </summary>
        public bool IsCanGetExtraLife()
        {
            return m_lifeRewardUsed < maxRewardLife;
        }

        void MissionComplete()
        {
            if (!m_player.IsDead)
            {
                string nextMissionId = MissionsManager.Ins.GetNextMissionId();

                if (Prefs.IsMissionUnlocked(nextMissionId) && string.Compare(nextMissionId, Prefs.currentMission) != 0)
                {
                    m_coinsBonus = Mathf.CeilToInt(MissionsManager.Ins.CoinsBonus / (m_player.stats.level * 10)) * m_curLife;
                    m_xpBonus = Mathf.CeilToInt(MissionsManager.Ins.XpBonus / (m_player.stats.level * 10));
                }
                else
                {
                    m_coinsBonus = MissionsManager.Ins.CoinsBonus * m_curLife;
                    m_xpBonus = MissionsManager.Ins.XpBonus;
                }

                Prefs.coins += m_coinsBonus ;

                Prefs.UnlockMission(nextMissionId, true);

                m_player.AddXp(m_xpBonus);
                m_player.SaveData();

                GameUIManager.Ins.UpdateCoinsInfo();

                Timer.Schedule(this, 5f, () =>
                {
                    AudioController.Ins.StopPlayMusic();
                    AudioController.Ins.PlaySound(AudioController.Ins.victorySound);
                    DialogController.Ins.ShowDialog(DialogType.MissionCompleted, DialogShow.REPLACE_CURRENT);
                    GamePadsController.Ins.Disable();
                });
            }
        }

        private void Update()
        {
            if(m_curLife <= 0 && !m_isGameover)
            {
                if (OnGameover != null)
                    OnGameover.Invoke();

                m_isGameover = true;

                if (IsCanGetExtraLife())
                {
                    Timer.Schedule(this, 1f, () =>
                    {
                        DialogController.Ins.ShowDialog(DialogType.ExtraLife, DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
                    });
                }
                else
                {
                    GameUIManager.Ins.ShowGameGui(false);
                    GameUIManager.Ins.ShowGameoverText(true);
                    CUtils.ShowInterstitialAd();
                }
            }
        }
    }
}
