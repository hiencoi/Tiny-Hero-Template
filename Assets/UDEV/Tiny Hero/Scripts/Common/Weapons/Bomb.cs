using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.AI2D;
using UDEV.SPM;

namespace UDEV.TinyHero
{
    public class Bomb : DamageCreater
    {
        [Header("Base Settings:")]
        public float explosionTime;
        public FlashVfx flashVfx;
        public float flashWhen;
        float m_curTime;
        bool m_countingBegun;

        [PoolerKeys(target = PoolerTarget.VFX)]
        public string explosionVfxPool;

        private void Update()
        {
            if (m_countingBegun)
            {
                m_curTime += Time.deltaTime;

                if ((explosionTime - m_curTime) <= flashWhen)
                    if (flashVfx)
                        flashVfx.Flash(flashWhen);
            }
        }

        private void OnEnable()
        {
            DamageDealed = false;
        }

        private void OnDisable()
        {
            if (flashVfx)
            {
                flashVfx.StopFlash();
                flashVfx.SetSpritesAlpha(flashVfx.normalColor);
            }

            m_countingBegun = false;
            m_curTime = 0;
        }

        public override void DealDamage()
        {
            if (!gameObject.activeInHierarchy) return;

            m_countingBegun = true;

            Timer.Schedule(this, explosionTime, () =>
            {
                Explode();
            });
        }

        public override void DealDamageWhenAffected()
        {
            StopAllCoroutines();
            Explode();
        }

        public void Explode()
        {
            DamageDealed = true;

            var expVfx = PoolersManager.Ins.Spawn(PoolerTarget.VFX, explosionVfxPool, transform.position, Quaternion.identity);
            var dmgCreaterComp = expVfx.GetComponent<DamageCreater>();

            if (dmgCreaterComp)
            {
                dmgCreaterComp.Init(damage, dealDamageTo, gameObject);
                dmgCreaterComp.DealDamage();
            }

            gameObject.SetActive(false);
        }
    }
}