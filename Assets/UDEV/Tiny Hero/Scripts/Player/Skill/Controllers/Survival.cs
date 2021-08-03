using System.Collections;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class Survival : Skill
    {
        [Header("Skill Settings:")]
        public SurvialStats stats;

        bool m_recoverStarted;
        Coroutine m_recoverCo;

        protected override void Init()
        {
            autoRun = true;

            OnTriggerBegin.AddListener(() =>
            {
                m_recoverStarted = true;
                m_recoverCo = StartCoroutine(AutoRecover());
            });
        }

        public override void SetData()
        {
            Data = stats;
        }

        IEnumerator AutoRecover()
        {
            yield return new WaitForSeconds(1f);
            m_player.CurHealth += stats.recoveryRate;
            m_recoverCo = StartCoroutine(AutoRecover());
        }

        protected override void FinalResetCore()
        {
            m_recoverStarted = false;
            if (m_player && m_recoverCo != null)
            {
                StopCoroutine(m_recoverCo);
                m_recoverCo = null;
            }
        }

        protected override void RecoverCore()
        {
            base.RecoverCore();

            m_recoverStarted = true;
            m_recoverCo = StartCoroutine(AutoRecover());
        }
    }
}
