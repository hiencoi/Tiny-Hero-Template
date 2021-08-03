using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace UDEV.AI2D
{
    public class AirThrowBodyAttack : aiFeature
    {
        [Header("Feature Settings:")]
        public float distCondition; // Distance feature can be trigger
        public float minDistCondition; //Min Distance feature can be trigger
        public List<DamageCreater> weapons;
        public DamageTo damageTo;
        bool m_haveAttackPos;
        Vector3 m_attackPosition;
        bool m_playerReached;

        protected override void Init()
        {
            float bodyDmg = m_aiController.bodyDamage.GetValue();

            WeaponsInit(bodyDmg, damageTo);
        }

        void WeaponsInit(float dmg, DamageTo damageTo)
        {
            m_aiController.WeaponsInit(weapons, dmg, damageTo, true);

            m_aiController.WeaponsInit(dmg, damageTo, true);
        }

        public override void OnAnimTrigger()
        {
            m_aiController.WeaponsTrigger(weapons);
            m_aiController.WeaponsTrigger();
        }

        protected override bool TriggerCondition()
        {
            return m_aiController.DistToPlayer <= distCondition && m_aiController.DistToPlayer > minDistCondition;
        }

        protected override void Core()
        {
            if(m_playerReached)
            {
                m_aiController.damage = damage.GetValue();
                WeaponsInit(damage.GetValue(), damageTo);
            }
        }

        protected override void OnTriggerUpdate()
        {
            if (!m_haveAttackPos)
            {
                m_attackPosition = m_aiController.Player.transform.position;
                m_haveAttackPos = true;
            }

            if (m_aiController && m_haveAttackPos)
            {
                if (Vector2.Distance(m_aiController.transform.position, m_attackPosition) > minDistCondition)
                {
                    m_aiController.transform.position = Vector2.MoveTowards(m_aiController.transform.position, m_attackPosition, m_aiController.CurSpeed * Time.deltaTime);
                }
            }

            if (Vector2.Distance(m_aiController.transform.position, m_attackPosition) <= minDistCondition)
            {
                m_playerReached = true;
            }
        }

        protected override bool IsCanPlayAnimation()
        {
            return m_playerReached;
        }

        protected override bool IsCanReset()
        {
            return m_playerReached;
        }

        protected override void ResetToDefault()
        {
            base.ResetToDefault();

            m_haveAttackPos = false;

            m_playerReached = false;

            float bodyDmg = m_aiController.bodyDamage.GetValue();

            WeaponsInit(bodyDmg, damageTo);
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.root.position,
                transform.root.position + new Vector3(0f, -distCondition, 0f));
        }
    }
}
