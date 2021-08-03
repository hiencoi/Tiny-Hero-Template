using UnityEngine;

namespace UDEV.TinyHero
{
    public class MissionsManager : Singleton<MissionsManager>
    {
        public Player player;
        public MissionController startingMission;

        string m_curMissionId;
        int m_missionIndex;
        int m_nextMissionId;

        [System.Serializable]
        public class Mission
        {
            public int missionOrder;
            public MissionController missionController;
            [Tooltip("Bonus per player life left")]
            public int minCoinsBonus;
            public int maxCoinsBonus;
            public int minXpBonus;
            public int maxXpBonus;
        }
        public Mission[] missions;

        Mission m_curMission;
        Player m_curPlayer;
        MissionController m_controller;

        public Mission CurMission { get => m_curMission; set => m_curMission = value; }
        public MissionController MissionController { get => m_controller; set => m_controller = value; }
        public string CurMissionId {
            get => m_curMissionId;
            set {
                Prefs.currentMission = value;
                m_curMissionId = value;
            }
        }

        public Player CurPlayer { get => m_curPlayer; set => m_curPlayer = value; }

        public int CoinsBonus { get => Random.Range(m_curMission.minCoinsBonus, m_curMission.maxCoinsBonus); }
        public int XpBonus { get => Random.Range(m_curMission.minXpBonus, m_curMission.maxXpBonus); }

        public override void Start()
        {
            base.Start();
            Prefs.UnlockMission(startingMission.id, true);
        }

        /// <summary>
        /// Instantiate Mission and Map
        /// </summary>
        public void Init()
        {
            SetMission();

            if (m_curMission != null)
            {
                m_controller = Instantiate(m_curMission.missionController, Vector3.zero, Quaternion.identity);
                
                if (m_controller.CurMap.playerSpawnPoint)
                {
                    m_curPlayer = Instantiate(player, m_controller.CurMap.playerSpawnPoint.position, Quaternion.identity);
                    m_curPlayer.LoadData();
                    GameManager.Ins.Player = CurPlayer;
                    GunShopData.Ins.Player = m_curPlayer;
                }
            }
        }

        void SetMission()
        {
            if (missions != null && missions.Length > 0)
            {
                for (int i = 0; i < missions.Length; i++)
                {
                    if (missions[i] != null &&
                        missions[i].missionController != null
                        && string.Compare(m_curMissionId, missions[i].missionController.id) == 0)
                    {
                        m_curMission = missions[i];
                        m_missionIndex = i;
                        break;
                    }
                }
            }
        }

        public string GetNextMissionId()
        {
            if ((m_missionIndex + 1) < missions.Length &&
                missions[m_missionIndex + 1] != null &&
                missions[m_missionIndex + 1].missionController != null)
            {
                return missions[m_missionIndex + 1].missionController.id;
            }
            return m_curMission.missionController.id;
        }

        public int GetCoinsBonus()
        {
            if (m_curMission != null)
                return Random.Range(m_curMission.minCoinsBonus, m_curMission.maxCoinsBonus);
            return 0;
        }

        public int GetXpBonus()
        {
            if (m_curMission != null)
                return Random.Range(m_curMission.minXpBonus, m_curMission.maxXpBonus) * (m_missionIndex + 1);
            return 0;
        }
    }
}
