using UnityEngine;
using System.Collections;

namespace UDEV.AI2D
{
    public class aiFeature : MonoBehaviour
    {
        [Header("Base Settings:")]
        [Range(0f, 1f)]
        public float triggerRate;
        [Range(0f, 180f)]
        public float triggerField = 45;
        public bool isPassive;
        public float moveSpeed; //Move speed of ai when feature trigger
        public Stat damage;
        [UniqueId]
        public string id; //Feature Id
        [AnimatorStates]
        public AnimState animatorState; // Animator state name when feature trigger

        protected float m_animStateLength; // Lenght of Animator state
        protected float m_totalTimeTrigger;
        protected float m_curTimeTrigger;
        float m_timeTriggerCounting;

        public float timeTrigger; // Time delay when feature trigger complete

        [Header("Next Feature Settings:")]
        [aiFeatures]
        public string nextFeature;
        public bool triggerOnEnter;
        public bool triggerOnUpdate;
        public bool triggerOnExit;
        protected aiBase m_aiController; // aiBase script
        Animator m_aiAim; //Animator component
        protected aiFeaturesManager m_ftMng;
        aiFeature m_nextFeature, m_featureBelong;

        bool m_isActive; // Feature running or not
        bool m_canTrigger;
        bool m_exitImmediately = true;
        bool m_isResettingInprocess;

        protected Coroutine m_resetDelay;
        protected Coroutine m_triggerRateRefresh;

        public bool IsActive { get => m_isActive; set => m_isActive = value; }
        public aiFeature NextFeature
        {
            get
            {
                if (m_ftMng && m_nextFeature == null)
                    m_nextFeature = m_ftMng.GetFeatureById(nextFeature);

                return m_nextFeature;
            }
            set => m_nextFeature = value;
        }
        public bool CanTrigger { get => m_canTrigger; set => m_canTrigger = value; }
        public bool ExitImmediately { get => m_exitImmediately; set => m_exitImmediately = value; }

        /// <summary>
        /// Initialize data of feature
        /// </summary>
        /// <param name="aiController">AIController Script</param>
        public void Initialize(aiBase aiController)
        {
            m_aiController = aiController;

            m_aiAim = m_aiController.anim;

            m_ftMng = m_aiController.featuresManager;

            m_canTrigger = true;

            SetTimeTrigger();

            m_timeTriggerCounting = 0f;

            if (!isPassive)
                m_triggerRateRefresh = StartCoroutine(TriggerRateRefresh());

            m_aiController.ActiveFeature = null;
            m_aiController.FSM_StatesSwitch();

            m_isActive = false;

            m_ftMng.HasActiveFeature = false;

            m_isResettingInprocess = false;

            RemoveModifiers();

            Init();
        }

        protected virtual void Init()
        {

        }

        void SetTimeTrigger()
        {
            var state = Helper.GetClip(m_aiAim, animatorState.clipName);

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

        public bool IsReady()
        {
            if (m_canTrigger)
                return m_aiController.PlayerAngle <= triggerField && TriggerCondition();

            return false;
        }

        protected virtual bool TriggerCondition()
        {
            return true;
        }

        IEnumerator TriggerRateRefresh()
        {
            if (!isPassive)
            {
                if (m_triggerRateRefresh != null)
                    StopCoroutine(m_triggerRateRefresh);

                yield return null;
            }

            if (!m_canTrigger)
            {
                float check = Random.Range(0f, 1f);

                if (check <= triggerRate)
                    m_canTrigger = true;
                else
                    m_canTrigger = false;
            }

            yield return new WaitForSeconds(3f);

            m_triggerRateRefresh = StartCoroutine(TriggerRateRefresh());
        }

        /// <summary>
        /// Feature and Animator state excute
        /// </summary>
        public void Trigger()
        {
            //Check ai feature manager enable or not
            if (m_aiController.featuresManager.enabled)
            {
                ResetFeature();

                if (m_isActive)
                {
                    m_ftMng.ActiveFeature = this;

                    Core();

                    PlayAnimatorState();

                    if (m_featureBelong && m_featureBelong.IsActive && IsSameAnimState(m_featureBelong))
                    {
                        m_featureBelong.IsActive = false;
                        if (m_resetDelay != null)
                            StopCoroutine(m_resetDelay);
                    }

                    if (NextFeature && triggerOnUpdate && NextFeature.IsReady())
                    {
                        ChangeFeature(NextFeature);
                    }

                    m_timeTriggerCounting += Time.deltaTime;

                    if (m_timeTriggerCounting > (m_totalTimeTrigger + 0.1) && m_isResettingInprocess && m_exitImmediately)
                    {
                        m_timeTriggerCounting = 0f;
                        ExitFeature();
                    }
                }
            }

            OnTriggerUpdate();
        }

        /// <summary>
        /// Core of feature ( all code of feature in here )
        /// </summary>
        protected virtual void Core()
        {

        }

        protected virtual void OnTriggerEnter()
        {

        }

        protected virtual void OnTriggerUpdate()
        {

        }

        protected virtual void OnTriggerExit()
        {

        }

        public virtual void OnAnimTrigger()
        {

        }

        /// <summary>
        /// Play animator state
        /// </summary>
        protected void PlayAnimatorState()
        {
            if (m_aiController && string.Compare(animatorState.name, "None") != 0 && IsCanPlayAnimation())
            {

                Helper.PlayAnimatorState(m_aiAim, animatorState.layerIndex, animatorState.name);
            }
        }

        protected virtual bool IsCanPlayAnimation()
        {
            return true;
        }

        protected bool IsSameAnimState(aiFeature feature)
        {
            return Helper.IsAnimatorStateActive(m_aiAim, feature.animatorState.layerIndex, feature.animatorState.name);
        }

        /// <summary>
        /// Reset feature to default
        /// </summary>
        protected virtual void ResetToDefault()
        {

        }

        protected virtual void FinalReset()
        {

        }

        /// <summary>
        /// Delay when feature excute finish and
        /// reset feature to default
        /// </summary>
        protected void ResetFeature()
        {
            if (!m_ftMng.HasActiveFeature && IsCanReset() && !m_isResettingInprocess)
            {
                m_timeTriggerCounting = 0f;

                if (NextFeature && triggerOnEnter)
                    ChangeFeature(NextFeature);

                OnTriggerEnter();

                m_curTimeTrigger = m_totalTimeTrigger;

                m_ftMng.HasActiveFeature = true;

                m_isActive = true;

                m_resetDelay = StartCoroutine(ResetDelayCo());
            }
        }

        protected virtual bool IsCanReset()
        {
            return true;
        }

        /// <summary>
        /// Delay when feature excute finish Coroutine
        /// </summary>
        /// 
        protected IEnumerator ResetDelayCo()
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
                    ExitFeature();

                    break;
                }

                if (m_featureBelong)
                    m_featureBelong.ResetToDefault();

                if (m_nextFeature)
                    m_nextFeature.ResetToDefault();

                ResetToDefault();
            }
        }

        protected void ExitFeature()
        {
            if (NextFeature && triggerOnExit)
                ChangeFeature(NextFeature);
            else
            {
                if (m_exitImmediately)
                {
                    m_canTrigger = false;
                    m_aiController.ActiveFeature = null;
                    m_aiController.FSM_StatesSwitch();
                }
                else if (!IsReady())
                {
                    m_canTrigger = false;
                    m_aiController.ActiveFeature = null;
                    m_aiController.FSM_StatesSwitch();
                }
            }

            m_isActive = false;

            if (m_featureBelong)
            {
                m_featureBelong.ResetToDefault();
                m_featureBelong = null;
            }

            if (m_nextFeature)
            {
                m_nextFeature.ResetToDefault();
                m_nextFeature = null;
            }

            ResetToDefault();

            FinalReset();

            m_ftMng.HasActiveFeature = false;

            OnTriggerExit();

            m_curTimeTrigger = m_totalTimeTrigger;

            m_isResettingInprocess = false;

            m_timeTriggerCounting = 0f;
        }

        public void ChangeFeature(aiFeature feature)
        {
            if (feature == null) return;

            if (m_ftMng.ActiveFeature != null)
            {
                if (string.Compare(feature.id, m_aiController.ActiveFeature.id) != 0)
                {
                    if (!IsSameAnimState(feature))
                    {
                        if (m_resetDelay != null)
                            StopCoroutine(m_resetDelay);
                        m_aiController.ActiveFeature.IsActive = false;
                    }

                    feature.CanTrigger = true;
                    m_aiController.featuresManager.HasActiveFeature = false;
                    m_aiController.ActiveFeature.NextFeature = feature;
                    feature.m_featureBelong = m_aiController.ActiveFeature;
                    m_aiController.ActiveFeature = feature;
                }
                else
                {
                    m_aiController.ActiveFeature = null;
                    m_aiController.FSM_StatesSwitch();
                }
            }
            else
            {
                feature.CanTrigger = true;
                m_aiController.featuresManager.HasActiveFeature = false;
                m_aiController.ActiveFeature = feature;
            }
        }

        public virtual void RemoveModifiers()
        {
            damage.RemoveModifiers();
        }

        /// <summary>
        /// Draw visual on Gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {

        }
    }
}
