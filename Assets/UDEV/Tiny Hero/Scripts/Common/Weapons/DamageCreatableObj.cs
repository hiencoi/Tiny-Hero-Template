using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.SPM;

namespace UDEV.AI2D {
    public class DamageCreatableObj : DamageCreater
    {
        [Header("Object Settings:")]
        public float damageRange;
        public Transform damagePoint;
        public bool canThrow;

        [Header("VFX Setting:")]
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string obstacleHitVFXPool;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string bodyHitVFXPool;

        GameObject m_damagedTarget;
        Rigidbody2D m_rb;
        RaycastHit2D m_findedPoint;

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody2D>();
        }

        public override void DealDamage()
        {
            if (damagePoint == null) return;

            Collider2D[] findeds = Physics2D.OverlapCircleAll(damagePoint.position, damageRange);

            for (int i = 0; i < findeds.Length; i++)
            {
                if (DealCondition(findeds[i].gameObject) || 
                    findeds[i].gameObject.CompareTag(obstacleTag)
                    && findeds[i].gameObject.transform != transform)
                {
                    m_damagedTarget = findeds[i].gameObject;
                    break;
                }
            }

            if (m_damagedTarget == null) return;

            if (!canThrow)
            {
                OnHit = () =>
                {
                    Vector3 dir = m_damagedTarget.transform.position - damagePoint.position;
                    m_findedPoint = Physics2D.Raycast(damagePoint.position, dir, damageRange * 1.5f);
                    PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, bodyHitVFXPool, m_findedPoint.point, transform.localScale.x < 0 ? Quaternion.Inverse(transform.rotation) : transform.rotation);
                    m_findedPoint = new RaycastHit2D();
                    m_damagedTarget = null;
                };

                ApplyDamage(m_damagedTarget);
            }
            else
            {
                OnObstacleHit = () =>
                {
                    Vector3 dir = m_damagedTarget.transform.position - damagePoint.position;
                    m_findedPoint = Physics2D.Raycast(damagePoint.position, dir, damageRange);

                    if (m_findedPoint.collider)
                    {
                        PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, obstacleHitVFXPool, m_findedPoint.point, transform.localScale.x < 0 ? Quaternion.Inverse(transform.rotation) : transform.rotation);
                        if (!IsHolding && m_rb && Mathf.Abs(m_rb.velocity.y) > 0)
                        {
                            gameObject.SetActive(false);
                        }
                    }

                    m_damagedTarget = null;
                    m_findedPoint = new RaycastHit2D();
                };

                OnHit = () =>
                {
                    Vector3 dir = m_damagedTarget.transform.position - damagePoint.position;
                    m_findedPoint = Physics2D.Raycast(damagePoint.position, dir, damageRange);

                    if (m_findedPoint.collider)
                    {
                        PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, bodyHitVFXPool, m_findedPoint.point, transform.localScale.x < 0 ? Quaternion.Inverse(transform.rotation) : transform.rotation);
                        if (!IsHolding && m_rb && Mathf.Abs(m_rb.velocity.y) > 0)
                        {
                            gameObject.SetActive(false);
                        }
                    }

                    m_damagedTarget = null;
                    m_findedPoint = new RaycastHit2D();
                };

                ApplyDamage(m_damagedTarget);
            }
        }

        private void Update()
        {
            if (canThrow && !IsHolding)
            {
                DealDamage();
            }
        }

        /// <summary>
        /// Draw some visual on Gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (damagePoint == null) return;

            Gizmos.color = new Color32(90, 230, 255, 50);
            Gizmos.DrawSphere(damagePoint.position, damageRange);
        }
    }
}

