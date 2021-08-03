using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UDEV.AI2D;

namespace UDEV.TinyHero {
    public class GameUIManager : Singleton<GameUIManager>
    {
        public GameObject gameGui;
        public Text coinsInfoText;
        public Text scoreInfoText;

        public Image gunHudIcon;
        public Text bulletsInfoText;
        public Transform lifeGirdInfo;
        public GameObject lifeIcon;
        public ImageFilled levelFilledInfo;
        public Text levelInfoText;
        public Text hpInfoText;
        public Text curWaveCouting;
        public Text totalWaveCounting;
        public GameObject waveCouting;
        public GameObject bossFightNotify;

        public SkillButton[] skillbtns;

        public Text gameoverText;

        bool m_waveCoutingShowed;

        public bool WaveCoutingShowed { set => m_waveCoutingShowed = value; }
       
        public override void Awake()
        {
            MakeSingleton(false);
        }

        public override void Start()
        {
            base.Start();

            var currentWave = MissionsManager.Ins.MissionController.waves[MissionsManager.Ins.MissionController.currentWave];

            MissionsManager.Ins.MissionController.
                AddEvent(
                MissionController.SpawnStates.BetweenWaves,
                () => ShowWaveCounting(
                    MissionsManager.Ins.MissionController.currentWave + 1,
                    MissionsManager.Ins.MissionController.wavesLength,
                    currentWave.isBossWave
                    )
                );
        }

        public void ShowGameGui(bool isShow)
        {
            if (gameGui)
                gameGui.SetActive(isShow);
        }

        public void UpdateCoinsInfo()
        {
            SetText(coinsInfoText, Prefs.coins.ToString());
        }

        public void UpdateScoreInfo(int score)
        {
            SetText(scoreInfoText, score.ToString());
        }

        public void UpdateGunHud(Sprite newHud)
        {
            if (gunHudIcon)
                gunHudIcon.sprite = newHud;
        }

        public void UpdateBulletsInfo(int current, int total)
        {
            SetText(bulletsInfoText, current + " /" + total);
        }

        public void UpdateLife(int lifes)
        {
            if (lifeGirdInfo)
            {
                int lifeIcons = lifeGirdInfo.childCount;

                if (lifeIcons > 0)
                {
                    for (int i = 0; i < lifeIcons; i++)
                    {
                        var lifeIcon = lifeGirdInfo.GetChild(i);

                        if (lifeIcon)
                            Destroy(lifeIcon.gameObject);
                    }
                }

                if (lifes > 0)
                {
                    for (int i = 0; i < lifes; i++)
                    {
                        if (lifeIcon)
                        {
                            var lifeIconClone = Instantiate(lifeIcon, Vector3.zero, Quaternion.identity);

                            lifeIconClone.transform.SetParent(lifeGirdInfo);
                            lifeIconClone.transform.localScale = Vector3.one;
                        }
                    }
                }
            }
        }

        public void UpdateLevelInfoText(int level, bool isMax = false)
        {
            if(isMax)
                SetText(levelInfoText, "level max");
            else
                SetText(levelInfoText, level.ToString());
        }

        public void UpdateLevelFilledInfo(float current, float total)
        {
            if (levelFilledInfo)
                levelFilledInfo.UpdateValue(current, total);
        }

        public void UpdateHpInfo(int current, int total)
        {
            SetText(hpInfoText, current + " / " + total);
        }

        void SetText(Text txt, string content)
        {
            if (txt)
                txt.text = content;
        }

        public SkillButton GetSkillBtn(string id)
        {
            if(skillbtns != null && skillbtns.Length > 0)
            {
                for (int i = 0; i < skillbtns.Length; i++)
                {
                    if(skillbtns[i] != null && string.Compare(skillbtns[i].skill, id) == 0)
                    {
                        return skillbtns[i];
                    }
                }
            }
            return null;
        }

        IEnumerator ShowSkillBtnsListener(List<Skill> selectedSkills)
        {
            int count = 0;

            while(count <= 10)
            {
                if (GameManager.Ins.IsGamebegin)
                {
                    if (selectedSkills != null && selectedSkills.Count > 0)
                    {
                        foreach (Skill s in selectedSkills)
                        {
                            var skillBtn = GetSkillBtn(s.id);

                            if (skillBtn)
                                skillBtn.gameObject.SetActive(true);
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void ShowSkillBtns(List<Skill> selectedSkills)
        {
            StartCoroutine(ShowSkillBtnsListener(selectedSkills));
        }

        public void ShowGameoverText(bool isShow)
        {
            if (gameoverText)
            {
                gameoverText.gameObject.SetActive(isShow);

                Timer.Schedule(this, 3.5f, () =>
                {
                    CUtils.LoadScene(1, true);
                });
            }
        }

        public void ShowWaveCounting(int current, int total, bool isBossWave = false)
        {
            if (isBossWave)
            {
                if(bossFightNotify)
                {
                    var anim = bossFightNotify.GetComponent<Animator>();

                    if (!m_waveCoutingShowed)
                    {
                        m_waveCoutingShowed = true;

                        bossFightNotify.transform.localScale = Vector3.zero;

                        bossFightNotify.SetActive(true);

                        Timer.Schedule(this, 0.5f, () =>
                        {
                            bossFightNotify.transform.localScale = Vector3.one;

                            if (anim)
                            {
                                anim.SetTrigger("show");

                                Timer.Schedule(this, 0.5f, () =>
                                {
                                    anim.SetTrigger("hide");

                                    Timer.Schedule(this, 1.8f, () =>
                                    {
                                        bossFightNotify.SetActive(false);

                                        float timeBetweenWaves = MissionsManager.Ins.MissionController.timeBetweenWaves;

                                        Timer.Schedule(this, timeBetweenWaves, () =>
                                        {
                                            m_waveCoutingShowed = false;
                                        });
                                    });
                                });
                            }
                        });
                    }
                }
            }
            else
            {
                if (curWaveCouting && totalWaveCounting && waveCouting)
                {
                    if (!m_waveCoutingShowed)
                    {
                        m_waveCoutingShowed = true;

                        if (curWaveCouting)
                            curWaveCouting.text = (MissionsManager.Ins.MissionController.currentWave + 1).ToString();

                        if (totalWaveCounting)
                            totalWaveCounting.text = (MissionsManager.Ins.MissionController.wavesLength).ToString();

                        var anim = waveCouting.GetComponent<Animator>();

                        waveCouting.transform.localScale = Vector3.zero;

                        waveCouting.SetActive(true);

                        Timer.Schedule(this, 0.5f, () =>
                        {
                            waveCouting.transform.localScale = Vector3.one;

                            if (anim)
                            {
                                anim.SetTrigger("show");

                                Timer.Schedule(this, 0.5f, () =>
                                {
                                    anim.SetTrigger("hide");

                                    Timer.Schedule(this, 1.8f, () =>
                                    {
                                        waveCouting.SetActive(false);

                                        float timeBetweenWaves = MissionsManager.Ins.MissionController.timeBetweenWaves;

                                        Timer.Schedule(this, timeBetweenWaves, () =>
                                        {
                                            m_waveCoutingShowed = false;
                                        });
                                    });
                                });
                            }
                        });
                    }
                }
            }
        }

        public void ReloadBtn()
        {
            GameManager.Ins.Player.Gun.Reload();
        }
    }
}
