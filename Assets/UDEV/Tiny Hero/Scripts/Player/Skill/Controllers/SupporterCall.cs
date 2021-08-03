using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    public class SupporterCall : Skill
    {
        public Supporter supporterPrefab;

        Supporter m_supporterClone;

        protected override void Init()
        {
            autoRun = true;

            OnTriggerBegin.AddListener(() =>
            {
                Timer.Schedule(this, 0.5f, () =>
                {
                    SupporterInstantiate();
                });
            });
        }

        public override void SetData()
        {
            Data = supporterPrefab.stats;
        }

        void SupporterInstantiate()
        {
            if (supporterPrefab)
            {
                m_supporterClone = Instantiate(supporterPrefab, Vector3.zero, Quaternion.identity);

                if (m_supporterClone)
                {
                    m_supporterClone.Initialize(m_player, this);
                    m_supporterClone.Born();
                }
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();

            if (m_supporterClone)
            {
                m_supporterClone.RemoveBonus();
                m_supporterClone.AddBonusToPlayer();
            }
        }

        protected override void FinalResetCore()
        {
            if (m_supporterClone) m_supporterClone.Die();
        }


        protected override void RecoverCore()
        {
            base.RecoverCore();

            Timer.Schedule(this, 0.5f, () =>
            {
                if (m_supporterClone) m_supporterClone.Born();
            });
        }
    }
}
