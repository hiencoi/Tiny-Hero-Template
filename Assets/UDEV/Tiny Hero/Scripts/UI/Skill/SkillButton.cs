using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero
{
    public class SkillButton : MonoBehaviour
    {
        [SkillIds]
        public string skill;

        public Image coolDownVisual;

        Skill m_sk;

        Button m_btnComp;

        private void Awake()
        {
            m_btnComp = GetComponent<Button>();
        }

        public void Init()
        {
            m_sk = GameManager.Ins.Player.skillsManager.GetSkillById(skill);

            if (m_sk)
            {
                UpdateCooldownVisual();

                m_sk.OnCoolDown.AddListener(() => OnCoolDownEvent());
                m_sk.OnCoolDownEnd.AddListener(() => OnCoolDownEndEvent());
            }
        }

        void OnCoolDownEvent()
        {
            UpdateCooldownVisual();

            if (m_btnComp)
            {
                m_btnComp.enabled = false;
            }
        }

        void UpdateCooldownVisual()
        {
            float fillRate = m_sk.CurCoolDownTime / m_sk.Data.cooldownTime;

            if (coolDownVisual && fillRate <= 1)
                coolDownVisual.fillAmount = fillRate;
        }

        void OnCoolDownEndEvent()
        {
            if (m_btnComp)
            {
                m_btnComp.enabled = true;
            }
        }

        public void TriggerSkill()
        {
            if (m_sk)
                m_sk.TriggerStart();
        }
    }
}
