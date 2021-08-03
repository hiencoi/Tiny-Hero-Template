using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.GridSlider;
using System.Linq;

namespace UDEV.TinyHero
{
    public class SkillsDialog : GridSliderDialog
    {
        List<Skill> m_data;
        Player m_player;
        int m_selecteds;
        List<Skill> m_activeSkill;

        public override void Show()
        {
            base.Show();
            m_player = GameManager.Ins.Player;
            m_activeSkill = new List<Skill>();
            m_data = m_player.skillsManager.Skills;
            DrawSlider(m_data.Count);
            Time.timeScale = 0f;
        }

        public override void Close()
        {
            base.Close();
            Time.timeScale = 1f;
            AudioController.Ins.PlayMusic(AudioController.Ins.backgroundMusics, true);
        }

        public override void OnDialogCompleteClosed()
        {
            base.OnDialogCompleteClosed();

            var currentWave = MissionsManager.Ins.MissionController.waves[MissionsManager.Ins.MissionController.currentWave];

            GameUIManager.Ins.ShowWaveCounting(
                MissionsManager.Ins.MissionController.currentWave + 1,
                MissionsManager.Ins.MissionController.wavesLength,
                currentWave.isBossWave
            );
            GameUIManager.Ins.WaveCoutingShowed = false;
            GameUIManager.Ins.ShowGameGui(true);

            GameManager.Ins.IsGamebegin = true;
        }

        public override void ItemHandle(GameObject gridItem, int index)
        {
            SkillDialogItem itemUI = gridItem.GetComponent<SkillDialogItem>();

            if (itemUI)
            {
                if(m_data != null && m_data.Count > 0)
                {
                    if(m_data[index] != null)
                    {
                        Skill skill = m_player.skillsManager.GetSkillById(m_data[index].id);

                        if (skill)
                        {
                            ItemUnlockedUI(itemUI, skill);
                        }

                        if (itemUI.infoBtn)
                        {
                            itemUI.infoBtn.onClick.RemoveAllListeners();
                            itemUI.infoBtn.onClick.AddListener(() => InfoBtnEvent(skill));
                        }

                        if (itemUI.selectBtn)
                        {
                            itemUI.selectBtn.onClick.RemoveAllListeners();
                            itemUI.selectBtn.onClick.AddListener(() => SelectBtnEvent(itemUI ,skill));
                        }
                    }
                }
            }
        }

        void BaseItemUI(SkillDialogItem itemUI, Skill skill)
        {
            itemUI.UpdateHudIcon(skill.hudIcon);
        }

        void ItemUnlockedUI(SkillDialogItem itemUI, Skill skill)
        {
            BaseItemUI(itemUI, skill);

            itemUI.hudIcon.color = Color.white;

            itemUI.selectBtn.enabled = true;

            itemUI.infoBtn.enabled = true;

            itemUI.infoBtnImage.color = new Color32(109, 219, 0, 255);

            itemUI.toggleBtn.gameObject.SetActive(true);
        }

        void ItemLockedUI(SkillDialogItem itemUI, Skill skill)
        {
            BaseItemUI(itemUI, skill);

            itemUI.hudIcon.color = Color.gray;

            itemUI.selectBtn.enabled = false;

            itemUI.infoBtn.enabled = false;

            itemUI.infoBtnImage.color = Color.gray;

            itemUI.toggleBtn.gameObject.SetActive(false);
        }

        void InfoBtnEvent(Skill skill)
        {
            DialogController.Ins.ShowDialog(DialogType.SkillInfo, DialogShow.OVER_CURRENT);

            var currentDialog = (SkillInfoDialog)DialogController.Ins.current;

            currentDialog.UpdateInfos(skill);

            Hide();
        }

        void SelectBtnEvent(SkillDialogItem itemUI ,Skill skill)
        {
            if(m_selecteds <= m_player.maxSkill)
            {
                itemUI.toggleBtn.isOn = !itemUI.toggleBtn.isOn;

                m_selecteds = itemUI.toggleBtn.isOn ? m_selecteds + 1 : m_selecteds - 1;

                if (itemUI.toggleBtn.isOn)
                    m_activeSkill.Add(skill);
                else
                    m_activeSkill.Remove(skill);

                m_player.skillsManager.ActiveSkills = m_activeSkill;

                if (m_selecteds == m_player.maxSkill || 
                    (m_data.Count <= m_player.maxSkill && m_selecteds == m_data.Count))
                {
                    if (m_activeSkill != null && m_activeSkill.Count > 0)
                    {
                        for (int i = 0; i < m_activeSkill.Count; i++)
                        {
                            if (m_activeSkill[i] != null)
                            {
                                m_activeSkill[i].isUnlocked = true;

                                if (!m_activeSkill[i].autoRun)
                                {
                                    var skillBtn = GameUIManager.Ins.GetSkillBtn(m_activeSkill[i].id);

                                    if (skillBtn)
                                    {
                                        skillBtn.gameObject.SetActive(true);
                                        skillBtn.Init();
                                    }
                                }
                                else
                                {
                                    m_activeSkill[i].IsActive = true;
                                }
                            }
                        }
                    }

                    GameUIManager.Ins.ShowSkillBtns(m_activeSkill);
                    Close();
                    m_player.skillsManager.AutoRunSkillsTrigger();
                }
            }
        }
    }
}
