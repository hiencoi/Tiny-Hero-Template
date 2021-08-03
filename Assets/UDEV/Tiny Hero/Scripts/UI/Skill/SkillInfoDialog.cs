using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace UDEV.TinyHero
{
    public class SkillInfoDialog : Dialog
    {
        Player m_player;

        public Text skillPointsText;

        public Image hudIcon;
        public Text descriptionText;

        public Transform baseValues;
        public Transform upValues;
        public SkillValueInfoUI baseValueUI;
        public SkillValueInfoUI upValueUI;

        public Button upBtn;
        public Image upBtnImage;
        public Text upBtnText;

        public Text cancelBtnText;

        public override void Show()
        {
            base.Show();
            m_player = GameManager.Ins.Player;
        }

        public override void Close()
        {
            base.Close();

            Time.timeScale = 1f;
        }

        public override void OnDialogCompleteClosed()
        {
            Time.timeScale = 0f;
            base.OnDialogCompleteClosed();
        }

        public void UpdateInfos(Skill skill)
        {
            if (skill)
            {
                if (skillPointsText && m_player)
                    skillPointsText.text = m_player.stats.skillPoints.ToString();

                if (hudIcon)
                    hudIcon.sprite = skill.hudIcon;

                if (descriptionText)
                    descriptionText.text = skill.desctiption;

                var infos = skill.Data.Infos;

                var statNames = infos.Keys.ToArray();

                var statValues = infos.Values.ToArray();

                if(upBtn)
                {
                    upBtn.onClick.RemoveAllListeners();
                    upBtn.onClick.AddListener(() => UpgradeEvent(skill));
                }

                if (upBtnText)
                    upBtnText.text = "UP [" + skill.Data.skillPoints + "]";

                if(baseValues)
                    ClearGrid(baseValues);

                if (upValues)
                    ClearGrid(upValues);

                for (int i = 0; i < statNames.Length; i++)
                {
                    var baseValue = statValues[i].Keys.ToArray()[0];
                    var upValue = statValues[i].Values.ToArray()[0];

                    if(baseValues && baseValueUI)
                    {
                        var bVal = Instantiate(baseValueUI, Vector3.zero, Quaternion.identity);
                        bVal.SetValueText(statNames[i] + " : " + baseValue);
                        bVal.transform.SetParent(baseValues);
                        bVal.transform.localScale = Vector3.one;
                    }

                    if(upValues && upValueUI)
                    {
                        var uVal = Instantiate(upValueUI, Vector3.zero, Quaternion.identity);
                        uVal.SetValueText(upValue);
                        uVal.transform.SetParent(upValues);
                        uVal.transform.localScale = Vector3.one;
                    }
                }

                if(!skill.Data.IsCanUpgrade(m_player.stats.skillPoints) || skill.Data.IsMaxLevel())
                {
                    if (upValues)
                        upValues.gameObject.SetActive(false);

                    if (upBtn)
                        upBtn.enabled = false;

                    if (upBtnImage)
                        upBtnImage.color = Color.gray;

                    if (skill.Data.IsMaxLevel())
                    {
                        upBtnText.text = "[ MAX ]";
                    }

                    if (cancelBtnText)
                        cancelBtnText.text = "CLOSE";
                }
                else
                {
                    if (upValues)
                        upValues.gameObject.SetActive(true);

                    if (upBtn)
                        upBtn.enabled = true;

                    if (upBtnImage)
                        upBtnImage.color = new Color32(64, 161, 0 , 255);

                    if (cancelBtnText)
                        cancelBtnText.text = "CANCEL";
                }
            }
        }

        void ClearGrid(Transform root)
        {
            if (root)
            {
                int childs = root.childCount;

                if (childs > 0)
                {
                    for (int i = 0; i < childs; i++)
                    {
                        var child = root.GetChild(i);

                        if (child)
                            Destroy(child.gameObject);
                    }
                }
            }
        }

        void UpgradeEvent(Skill skill)
        {
            if (skill)
                skill.Upgrade();
            AudioController.Ins.EnableAudio(true);
            AudioController.Ins.PlaySound(AudioController.Ins.popupOrUpgrade);
            UpdateInfos(skill);
        }
    }
}
