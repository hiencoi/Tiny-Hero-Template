using UnityEngine;
using UDEV.SPM;
using System;

namespace UDEV.AI2D
{
    public class Projectile : DamageCreater
    {
        [Header("Base Settings:")]
        public float speed;
        [HideInInspector]
        public float curSpeed;

        [Header("VFX Setting:")]
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string obstacleHitVFXPool;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string bodyHitVFXPool;

        Vector2 m_lastPos;
        RaycastHit2D m_hit;

        // Use this for initialization
        void Start()
        {
            // Reset last known position:
            RefreshLastPos();
        }

        // Update is called once per frame
        void Update()
        {
            // Move:
            transform.Translate(transform.right * curSpeed * Time.deltaTime, Space.World);

            DealDamage();

            RefreshLastPos();
        }

        public override void DealDamage()
        {
            m_hit = Physics2D.Raycast(m_lastPos, new Vector2(transform.position.x, transform.position.y) - m_lastPos, (new Vector2(transform.position.x, transform.position.y) - m_lastPos).magnitude);

            if (m_hit && m_hit.collider)
            {
                var col = m_hit.collider;

                if (!col) return;

                OnObstacleHit = () =>
                {
                    PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, obstacleHitVFXPool, m_hit.point, transform.localScale.x < 0 ? Quaternion.Inverse(transform.rotation) : transform.rotation);
                    m_hit = new RaycastHit2D();
                    gameObject.SetActive(false);
                };

                OnHit = () =>
                {
                    PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, bodyHitVFXPool, m_hit.point, transform.localScale.x < 0 ? Quaternion.Inverse(transform.rotation) : transform.rotation);
                    m_hit = new RaycastHit2D();
                    gameObject.SetActive(false); 
                };

                ApplyDamage(col.gameObject);  
            }
        }

        public void RefreshLastPos()
        {
            m_lastPos = new Vector2(transform.position.x, transform.position.y);
        }

        private void OnDisable()
        {
            m_hit = new RaycastHit2D();
        }
    }
}
