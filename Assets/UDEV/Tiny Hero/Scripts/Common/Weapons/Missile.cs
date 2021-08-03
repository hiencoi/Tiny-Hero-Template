using UnityEngine;
using UDEV.AI2D;
using UDEV.SPM;

namespace UDEV.TinyHero
{
    public class Missile : DamageCreater
    {
        [Header("Base Settings:")]
        public float speed;
        Vector3 moveDir;
        Vector3 prevDir;
        GameObject m_fireTarget;
        [PoolerKeys(target = PoolerTarget.VFX)]
        public string explodeVFXPool;
        float m_distOriginal;
        Vector3 m_spawnPos;
        Vector2 m_lastPos;
        bool m_canMove;
        bool m_canCheckHit;
        Vector3 m_dealDmgPos;

        private void OnEnable()
        {
            m_canMove = true;

            Timer.Schedule(this, 0.3f, () =>
            {
                m_canCheckHit = true;
            });
        }

        private void OnDisable()
        {
            transform.position = new Vector3(1000, 1000, 0);
            m_canMove = false;
        }

        void Start()
        {
            // Reset last known position:
            RefreshLastPos();
        }

        public void SetTarget(GameObject target)
        {
            m_fireTarget = target;

            m_spawnPos = transform.position;

            m_distOriginal = Vector2.Distance(m_spawnPos, m_fireTarget.transform.position);
        }

        private void Update()
        {
            if (m_fireTarget && m_canMove)
            {
                aiBase ai = m_fireTarget.GetComponent<aiBase>();
                moveDir = m_fireTarget.transform.position - transform.position;
                moveDir = prevDir = moveDir.normalized;
                if (!m_fireTarget.activeInHierarchy || ai.IsDead)
                {
                    m_fireTarget = null;
                    moveDir = prevDir;
                }
                float rotZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, rotZ);
            }

            if(m_canMove)
                transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

            if(m_canCheckHit)
                CheckHit();
        }

        void CheckHit()
        {
            RaycastHit2D hit = Physics2D.Raycast(m_lastPos, new Vector2(transform.position.x, transform.position.y) - m_lastPos, (new Vector2(transform.position.x, transform.position.y) - m_lastPos).magnitude);
            if (hit)
            {
                // Do something when we hit a player:
                if (hit.collider.CompareTag(enemyTag) ||
                    hit.collider.CompareTag(obstacleTag) ||
                    hit.collider.CompareTag(playerTag))
                {
                    m_canMove = false;
                    m_dealDmgPos = hit.collider.transform.position;
                    DealDamage();
                }
            }

            RefreshLastPos();

            var dist = Vector2.Distance(m_spawnPos, transform.position);

            if (m_distOriginal - dist <= 0.5f)
            {
                m_canMove = false;
                m_dealDmgPos = transform.position;
                DealDamage();
            }
        }

        public override void DealDamage()
        {
            Explode();
        }

        public override void DealDamageWhenAffected()
        {
            Explode();
        }

        public void Explode()
        {
            DamageDealed = true;

            m_canCheckHit = false;

            var vfx = PoolersManager.Ins.Spawn(PoolerTarget.VFX, explodeVFXPool, m_dealDmgPos, Quaternion.identity);

            DamageCreater dmCreaterComp = null;

            if (vfx)
                dmCreaterComp = vfx.GetComponent<DamageCreater>();

            if (dmCreaterComp)
            {
                dmCreaterComp.Init(damage, dealDamageTo, gameObject);
                dmCreaterComp.DealDamage();
            }

            gameObject.SetActive(false);
        }

        public void RefreshLastPos()
        {
            m_lastPos = new Vector2(transform.position.x, transform.position.y);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (m_canCheckHit)
            {
                m_canCheckHit = false;
                m_canMove = false;
                m_dealDmgPos = col.contacts[0].point;
                DealDamage();
            }
        }
    }
}
