using UnityEngine;
using UDEV.SPM;

namespace UDEV.AI2D
{
    public class DropObject : aiFeature
    {
        [Header("Feature Settings:")]
        public float distCondition; // Distance feature can be trigger
        public float minDistCondition; //Min Distance feature can be trigger
        public bool dynamicRigidbody;
        public Transform dropPoint;
        Vector3 m_dropDir;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string objectPool;
        public DamageTo damageTo;
        public float dropForce;
        public bool createObjFirst;
        public bool TriggerOneDirection;
        bool m_isHaveDropDir;
        GameObject obj;
        Collider2D m_collider2D;

        bool m_isObjectCreated;
        bool m_isDropped;

        protected override void Init()
        {
            if (createObjFirst)
                CreateObject();
        }

        public override void OnAnimTrigger()
        {
            if (TriggerOneDirection)
            {
                if (!m_isHaveDropDir)
                {
                    m_dropDir = m_aiController.Player.transform.position - dropPoint.position;
                    m_isHaveDropDir = true;
                }
            }
            else
            {
                m_dropDir = m_aiController.Player.transform.position - dropPoint.position;
                m_aiController.Flip(m_dropDir);
            }

            Drop(m_dropDir);
        }

        protected override bool TriggerCondition()
        {
            return m_aiController.DistToPlayer <= distCondition && m_aiController.DistToPlayer > minDistCondition;
        }

        void CreateObject()
        {
            if (!m_isObjectCreated)
            {
                obj = null;

                m_isObjectCreated = true;

                obj = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON ,objectPool, dropPoint.position, Quaternion.identity);
                if (obj)
                {
                    obj.transform.SetParent(dropPoint.transform);
                    obj.transform.localPosition = Vector3.zero;
                    m_collider2D = obj.GetComponent<Collider2D>();

                    if (m_collider2D)
                        m_collider2D.isTrigger = true;
                }

                var rb = obj.GetComponent<Rigidbody2D>();
                if (rb) rb.isKinematic = true;

                var dmgCreaterCmp = obj.GetComponent<DamageCreater>();

                if (dmgCreaterCmp)
                {
                    float bodyDmg = m_aiController.bodyDamage.GetValue();
                    dmgCreaterCmp.Init(bodyDmg, damageTo, m_aiController.gameObject, true);
                    m_aiController.Weapons.Add(dmgCreaterCmp);
                }
            }
        }

        public void Drop(Vector3 dropDirection)
        {
            if (!createObjFirst)
                CreateObject();

            if (m_isObjectCreated && obj && !m_isDropped)
            {
                dropDirection.Normalize();

                var objRb = obj.GetComponent<Rigidbody2D>();

                var destroyComp = obj.GetComponent<AutoDestroy>();

                obj.transform.SetParent(null);

                if (dynamicRigidbody && objRb)
                {
                    if (m_collider2D)
                        m_collider2D.isTrigger = false;

                    objRb.isKinematic = false;
                    objRb.drag = 1f;
                }

                if (objRb)
                    objRb.velocity = dropDirection.normalized * dropForce;

                if (destroyComp)
                {
                    if (destroyComp.isOverride)
                        destroyComp.DestroyTrigger();
                }

                var dmgCreaterCmp = obj.GetComponent<DamageCreater>();

                if (dmgCreaterCmp)
                {
                    dmgCreaterCmp.Init(damage.GetValue(), damageTo, m_aiController.gameObject.gameObject);
                    dmgCreaterCmp.DealDamage();
                    m_aiController.RemoveWeapon(dmgCreaterCmp);
                }

                m_isDropped = true;
            }
        }

        protected override void ResetToDefault()
        {
            base.ResetToDefault();

            if (m_isDropped)
            {
                m_isObjectCreated = false;
                m_isDropped = false;
                obj = null;
                if (createObjFirst)
                    CreateObject();
            }
        }

        protected override void FinalReset()
        {
            m_isHaveDropDir = false;
        }
    }
}
