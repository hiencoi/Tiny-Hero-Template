using UnityEngine;
using System.Collections.Generic;

namespace UDEV.AI2D
{
    public class WeaponAttack : aiFeature
    {
        [Header("Feature Settings:")]
        public float distCondition; // Distance feature can be trigger
        public float minDistCondition; //Min Distance feature can be trigger
        public List<DamageCreater> weapons;
        public DamageTo damageTo;

        protected override void Init()
        {
            float bodyDmg = m_aiController.bodyDamage.GetValue();

            WeaponsInit(bodyDmg, damageTo);

            ExitImmediately = false;
        }

        void WeaponsInit(float dmg, DamageTo damageTo)
        {
            m_aiController.WeaponsInit(weapons, dmg, damageTo);

            m_aiController.WeaponsInit(dmg, damageTo);
        }

        protected override bool TriggerCondition()
        {
            return m_aiController.DistToPlayer <= distCondition && m_aiController.DistToPlayer > minDistCondition;
        }

        public override void OnAnimTrigger()
        {
            if (!m_aiController.IsDead)
                WeaponsInit(damage.GetValue(), damageTo);
            else
                WeaponsInit(0, damageTo);

            m_aiController.WeaponsTrigger(weapons);
        }

        protected override void ResetToDefault()
        {
            float bodyDmg = m_aiController.bodyDamage.GetValue();

            WeaponsInit(bodyDmg, damageTo);
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.root.position - new Vector3(distCondition, 0f, 0f),
                transform.root.position + new Vector3(distCondition, 0f, 0f));
        }
    }
}
