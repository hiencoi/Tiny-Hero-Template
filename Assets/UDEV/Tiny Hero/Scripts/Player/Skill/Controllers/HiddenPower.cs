using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.SPM;

namespace UDEV.TinyHero
{
    public class HiddenPower : Skill
    {
        [Header("Skill Settings:")]
        public HiddenPowerStats stats;
        [PoolerKeys(target = PoolerTarget.VFX)]
        public string powerUpEffectPool;
        GameObject m_powerUpEffect;
        Coroutine healthRecoverCo;

        protected override void Init()
        {
            OnTriggerBegin.AddListener(() => PowerUp());
        }

        public override void SetData()
        {
            Data = stats;
        }

        void SpawnPowerUpEffect()
        {
            m_powerUpEffect = PoolersManager.Ins.Spawn(PoolerTarget.VFX ,powerUpEffectPool, Vector3.zero, Quaternion.identity);
            if (m_powerUpEffect)
            {
                m_powerUpEffect.transform.SetParent(m_player.transform.GetChild(0));
                m_powerUpEffect.transform.localScale = Vector3.one;
                m_powerUpEffect.transform.localPosition = Vector3.zero;
            }
        }

        void PowerUp()
        {
            GunController curGun = m_player.Gun;

            if (m_player && curGun)
            {
                curGun.stats.damage.AddModifier(stats.damageExtra);
                curGun.ReloadImmediately();
                m_player.CurInvincibleTime += stats.invincibleTime;
                healthRecoverCo = StartCoroutine(HealthRecover());
                m_player.Invincible(timeTrigger);
                SpawnPowerUpEffect();
                StartCoroutine(BackToNormalDelay());
            }
        }

        IEnumerator HealthRecover()
        {
            yield return new WaitForSeconds(1f);

            if (m_player)
                m_player.CurHealth += stats.healthRecovery;

            healthRecoverCo = StartCoroutine(HealthRecover());
        }

        IEnumerator BackToNormalDelay()
        {
            yield return new WaitForSeconds(stats.invincibleTime);

            BackToNormal();
        }

        void BackToNormal()
        {
            if (m_powerUpEffect)
                m_powerUpEffect.SetActive(false);

            GunController curGun = m_player.Gun;

            if (curGun && m_player)
            {
                curGun.stats.damage.RemoveModifier(stats.damageExtra);
                m_player.CurInvincibleTime = m_player.invincibleTime;
            }

            if (healthRecoverCo != null)
                StopCoroutine(healthRecoverCo);
        }

        protected override void FinalResetCore()
        {
            base.FinalResetCore();
            BackToNormal();
        }
    }
}
