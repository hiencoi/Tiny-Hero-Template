using UnityEngine;
using UDEV.SPM;

namespace UDEV.AI2D
{
    public class HealthRecover : aiFeature
    {
        [Header("Feature Settings:")]
        public int healthTrigger;
        public int healthRecover;

        [PoolerKeys(target = PoolerTarget.VFX)]
        public string healingVFXPool;
        GameObject m_vfx;

        protected override bool TriggerCondition()
        {
            return m_aiController.CurHealth <= healthTrigger;
        }

        protected override void OnTriggerEnter()
        {
            base.OnTriggerEnter();

            m_aiController.CurHealth += healthRecover;
            m_vfx = PoolersManager.Ins.Spawn(PoolerTarget.VFX, healingVFXPool, m_aiController.transform.position, Quaternion.identity);
            if (m_vfx)
            {
                m_vfx.transform.SetParent(m_aiController.transform);
                m_vfx.transform.localPosition = Vector3.zero;
            }
        }

        protected override void FinalReset()
        {
            base.FinalReset();

            m_vfx = null;
        }
    }
}
