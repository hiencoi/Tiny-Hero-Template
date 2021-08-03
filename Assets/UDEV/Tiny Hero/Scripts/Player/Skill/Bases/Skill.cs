using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UDEV.AI2D;
using System;
using System.IO;

namespace UDEV.TinyHero
{
    public class Skill : MonoBehaviour
    {
        public new string name;
        [Header("Base Settings:")]
        [UniqueId]
        public string id;
        public Sprite hudIcon;

        [AnimatorStates]
        public AnimState animState;
        public bool autoRun;

        float m_curCooldownTime;

        public float timeTrigger;
        protected float m_animStateLength; // Lenght of Animator state
        protected float m_totalTimeTrigger;
        protected float m_curTimeTrigger;
        float m_timeTriggerCounting;

        protected Player m_player;

        protected bool m_isInCoolDown;
        Coroutine m_coolDown;

        [TextArea]
        public string desctiption;

        [HideInInspector]
        public UnityEvent OnTriggerBegin;
        [HideInInspector]
        public UnityEvent OnTrigger;
        [HideInInspector]
        public UnityEvent OnTriggerEnd;
        [HideInInspector]
        public UnityEvent OnCoolDown;
        [HideInInspector]
        public UnityEvent OnCoolDownEnd;
        [HideInInspector]
        public UnityEvent OnFinalReset;

        SkillStats m_data;

        bool m_isActive;
        [HideInInspector]
        public bool isUnlocked;
        bool m_isResettingInprocess;

        public bool IsActive { get => m_isActive; set => m_isActive = value; }
        public float CurCoolDownTime { get => m_curCooldownTime; }
        public SkillStats Data { get => m_data; set => m_data = value; }

        /// <summary>
        /// Skill Initialize
        /// </summary>
        /// <param name="player">Player Script</param>
        public void SkillInit(Player player)
        {
            SetData();

            LoadData();

            m_data.Init();

            m_player = player;

            m_curCooldownTime = 0;

            m_isResettingInprocess = false;

            ResetToDefault();

            m_timeTriggerCounting = 0;

            Init();

            SetTimeTrigger();
        }

        protected virtual void Init()
        {

        }

        public virtual void SetData()
        {

        }

        void SetTimeTrigger()
        {
            var state = Helper.GetClip(m_player.anim, animState.clipName);

            if (state)
                m_animStateLength = state.length;

            m_totalTimeTrigger = m_animStateLength + timeTrigger;

            if (m_animStateLength > 0 && timeTrigger > 0)
            {
                var timeDiv = timeTrigger % m_animStateLength;

                m_totalTimeTrigger = m_animStateLength + timeTrigger + (m_animStateLength - timeDiv);
            }

            m_curTimeTrigger = m_totalTimeTrigger;
        }

        /// <summary>
        /// Skill Core
        /// </summary>
        public virtual void OnTriggerUpdate()
        {

        }

        public void AutoRunTrigger()
        {
            if (!autoRun) return;

            if (OnTriggerBegin != null)
                OnTriggerBegin.Invoke();

            AutoRunTriggerCore();
        }

        protected virtual void AutoRunTriggerCore()
        {

        }

        /// <summary>
        /// Trigger skill
        /// </summary>
        public void Trigger()
        {
            if (!isUnlocked) return;

            if (m_isActive)
            {
                Excute();

                m_timeTriggerCounting += Time.deltaTime;

                if (m_timeTriggerCounting > (m_totalTimeTrigger + 0.1) && m_isResettingInprocess)
                {
                    m_timeTriggerCounting = 0f;
                    ResetToDefault();
                }
            }
        }

        public virtual void OnAnimTrigger()
        {

        }

        /// <summary>
        /// Excute skill and animator state
        /// </summary>
        protected void Excute()
        {
            if((m_isInCoolDown && !autoRun) || autoRun)
            {
                if (OnTrigger != null)
                    OnTrigger.Invoke();

                OnTriggerUpdate();
            }
        }

        public void TriggerStart()
        {
            m_curTimeTrigger = m_totalTimeTrigger;

            m_timeTriggerCounting = 0f;

            if (!m_isActive && !m_player.skillsManager.HasAnySkillTriggered)
            {
                GameManager.Ins.Player.Fsm.ChangeState(Player.States.SkillActive);

                m_player.skillsManager.HasAnySkillTriggered = true;

                m_player.skillsManager.SkillTriggered = this;

                m_isActive = true;

                Delay();

                CoolDownStart();
            }
        }

        void CoolDownStart()
        {
            if (!m_isInCoolDown)
            {
                m_curCooldownTime = m_data.cooldownTime;

                m_curTimeTrigger = m_totalTimeTrigger;

                m_coolDown = StartCoroutine(CoolDownCo());

                m_isInCoolDown = true;
            }
        }

        /// <summary>
        /// Cool down coroutine
        /// </summary>
        IEnumerator CoolDownCo()
        {
            if (OnTriggerBegin != null && !m_isInCoolDown)
            {
                OnTriggerBegin.Invoke();
            }

            while (m_curCooldownTime > 0)
            {
                yield return new WaitForSeconds(1f);

                m_curCooldownTime -= m_data.coolDownRate;

                if (OnCoolDown != null)
                    OnCoolDown.Invoke();
            }

            if (m_curCooldownTime <= 0)
            {
                m_curCooldownTime = m_data.cooldownTime;

                m_isInCoolDown = false;

                if (OnCoolDownEnd != null)
                    OnCoolDownEnd.Invoke();
            }

            yield return null;
        }

        /// <summary>
        /// Load Skill Data from unity database
        /// </summary>
        void LoadData()
        {
            string json = CPlayerPrefs.GetString(GameConsts.SKILL_DATA + id, null);

            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, m_data);
            }
        }

        /// <summary>
        /// Save Skill Data from unity database
        /// </summary>
        void SaveData()
        {
            string json = JsonUtility.ToJson(m_data);
            CPlayerPrefs.SetString(GameConsts.SKILL_DATA + id, json);
        }

        /// <summary>
        /// Upgrade skill
        /// </summary>
        public virtual void Upgrade()
        {
            if (m_data.IsCanUpgrade(m_player.stats.skillPoints) && !m_data.IsMaxLevel())
            {
                m_player.stats.skillPoints -= m_data.skillPoints;

                m_player.SaveData();

                m_data.Upgrade();

                SaveData();

                LoadData();
            }
        }

        /// <summary>
        /// Reset skill to default
        /// </summary>
        protected void ResetToDefault()
        {
            if (OnTriggerEnd != null)
                OnTriggerEnd.Invoke();

            m_isActive = false;

            m_player.skillsManager.HasAnySkillTriggered = false;

            m_player.skillsManager.SkillTriggered = null;

            m_player.Fsm.ChangeState(Player.States.Idle);

            ResetToDefaultCore();
        }

        protected virtual void ResetToDefaultCore()
        {

        }

        /// <summary>
        /// Delay when skill excute finish
        /// </summary>
        public void Delay()
        {
            StartCoroutine(ResetCo());
        }

        /// <summary>
        /// Delay when skill excute finish Coroutine
        /// </summary>
        IEnumerator ResetCo()
        {
            m_isResettingInprocess = true;

            float delay = timeTrigger;

            if (m_animStateLength > 0)
            {
                delay = m_animStateLength;
            }

            int count = 0;

            while (m_curTimeTrigger > 0)
            {
                yield return new WaitForSeconds(delay);

                m_curTimeTrigger -= delay;

                count++;

                if (m_curTimeTrigger <= 0 || count > 10)
                {
                    ResetToDefault();

                    break;
                }
            }
            ResetToDefault();
        }

        public void FinalReset()
        {
            if (OnFinalReset != null)
                OnFinalReset.Invoke();

            FinalResetCore();
        }

        public void Recover()
        {
            if (m_isInCoolDown)
                m_coolDown = StartCoroutine(CoolDownCo());

            RecoverCore();
        }

        protected virtual void RecoverCore()
        {

        }

        protected virtual void FinalResetCore()
        {

        }
    }
}
