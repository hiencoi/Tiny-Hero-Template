using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class FreezeEnemies : Skill
    {
        [Header("Skill Settings:")]
        public FreezeEnemiesStats stats;
        public AudioClip beepSound;

        Coroutine m_reduceFreezeTimeCo;
        Coroutine m_playBeepSoundCo;

        protected override void Init()
        {
            OnTriggerBegin.AddListener(() => Freeze());
        }

        public override void SetData()
        {
            Data = stats;
        }


        void Freeze()
        {
            MissionController missionController = MissionsManager.Ins.MissionController;

            if (missionController)
                missionController.PauseWave();

            m_playBeepSoundCo = StartCoroutine(PlayBeepSound());

            StartCoroutine(BackToNormalDelay());
        }

        IEnumerator PlayBeepSound()
        {
            int count = 0;
            while (count < stats.freezeTime)
            {
                AudioController.Ins.PlaySound(beepSound);
                yield return new WaitForSeconds(1f);
                count++;
            }
        }

        IEnumerator BackToNormalDelay()
        {
            yield return new WaitForSeconds(stats.freezeTime);

            BackToNormal();
        }

        void BackToNormal()
        {
            MissionController missionController = MissionsManager.Ins.MissionController;

            if (missionController)
                missionController.ResumeWave();

            if (m_playBeepSoundCo != null)
                StopCoroutine(m_playBeepSoundCo);

            if (OnTriggerEnd != null)
                OnTriggerEnd.Invoke();
        }

        protected override void ResetToDefaultCore()
        {
            BackToNormal();
        }

        protected override void FinalResetCore()
        {
            BackToNormal();
        }
    }
}
