using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UDEV.SPM;
using UDEV.AI2D;

namespace UDEV.TinyHero
{
    public class Supporter : DamageCreater
    {
        [Header("Base Settings:")]
        public Facing facing;
        public SupporterCallStats stats;
        public float moveSpeed;
        public float searchRange;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string projectilePool;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string muzzleVFXPool;
        public Transform shootingPoint;

        [Header("Spawn Settings:")]
        public float playerOffsetY;
        public float playerOffsetX;

        Vector3 movePos;
        Coroutine attackCo;
        bool m_attackRefreshed;
        List<GameObject> m_targets;
        Player m_player;
        Skill m_skill;

        public void Initialize(Player player, Skill skill)
        {
            m_player = player;
            m_skill = skill;
        }

        private void Update()
        {
            Vector3 gunDir = GamePadsController.Ins.ShootDir;

            Move(gunDir);

            Flip(gunDir);

            if (!m_player)
                Die();
        }

        void Flip(Vector3 flipDir)
        {
            switch (facing)
            {
                case Facing.LEFT:
                    Helper.FaceLeftFlip(flipDir, transform);
                    break;
                case Facing.RIGHT:
                    Helper.FaceRightFlip(flipDir, transform);
                    break;
            }
        }

        void StartingMoving()
        {
            if (!m_player) return;

            switch (m_player.facing)
            {
                case Facing.LEFT:
                    Flip(Vector3.left);
                    if(m_player.transform.localScale.x > 0)
                    {
                        movePos = new Vector3(
                        m_player.transform.position.x + playerOffsetX,
                        m_player.transform.position.y + playerOffsetY,
                        m_player.transform.position.z);
                    }
                    else
                    {
                        movePos = new Vector3(
                        m_player.transform.position.x - playerOffsetX,
                        m_player.transform.position.y + playerOffsetY,
                        m_player.transform.position.z);
                    }
                    break;
                case Facing.RIGHT:
                    Flip(Vector3.right);
                    if (m_player.transform.localScale.x > 0)
                    {
                        movePos = new Vector3(
                        m_player.transform.position.x - playerOffsetX,
                        m_player.transform.position.y + playerOffsetY,
                        m_player.transform.position.z);
                    }
                    else
                    {
                        movePos = new Vector3(
                        m_player.transform.position.x + playerOffsetX,
                        m_player.transform.position.y + playerOffsetY,
                        m_player.transform.position.z);
                    }
                    break;
            }
        }

        void Move(Vector3 moveDir)
        {
            if (!m_player) return;

            if(moveDir.x > 0)
            {
                movePos = new Vector3(
                m_player.transform.position.x - playerOffsetX,
                m_player.transform.position.y + playerOffsetY,
                m_player.transform.position.z);
            }
            else if(moveDir.x < 0)
            {
                movePos = new Vector3(
                m_player.transform.position.x + playerOffsetX,
                m_player.transform.position.y + playerOffsetY,
                m_player.transform.position.z);
            }
            else
            {
                StartingMoving();
            }

            transform.position = Vector3.MoveTowards(transform.position, movePos, moveSpeed * Time.deltaTime);
        }

        IEnumerator Attack()
        {
            yield return new WaitForSeconds(stats.fireRate);

            float check = Random.Range(0f, 1f);

            if (check <= stats.attackRate)
            {
                DealDamage();
            }

            attackCo = StartCoroutine(Attack());
        }

        void FindTarget()
        {
            m_targets = new List<GameObject>();

            Collider2D[] objectsFinded = Physics2D.OverlapCircleAll(transform.position, searchRange);

            var temps = new List<GameObject>();

            var roots = new List<GameObject>();

            if (objectsFinded != null && objectsFinded.Length > 0)
            {
                foreach (Collider2D check in objectsFinded)
                {
                    if (check.CompareTag(enemyTag))
                    {
                        temps.Add(check.gameObject);
                    }
                }
            }

            if (temps != null && temps.Count > 0)
            {
                for (int i = 0; i < temps.Count; i++)
                {
                    if (temps[i] != null)
                    {
                        var root = temps[i].transform.root;
                        if (root) roots.Add(root.gameObject);
                    }
                }
            }

            if (roots != null && roots.Count > 0)
            {
                m_targets = new List<GameObject>();
                m_targets = roots.GroupBy(r => r.GetInstanceID()).Select(r => r.First()).ToList();
            }
        }

        public override void DealDamage()
        {
            FindTarget();

            if (m_targets != null && m_targets.Count > 0)
            {
                int randIdx = Random.Range(0, m_targets.Count);

                GameObject target = m_targets[randIdx];

                if (target)
                {
                    Vector3 attackDir = target.transform.position - shootingPoint.transform.position;
                    attackDir.Normalize();

                    var rotZValue = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

                    Projectile p = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, projectilePool, shootingPoint.position, Quaternion.identity).GetComponent<Projectile>();

                    if (p)
                    {
                        p.transform.rotation = Quaternion.Euler(0f, 0f, rotZValue);
                        Vector3 pScale = p.transform.localScale;

                        float speedMult = 1;

                        p.curSpeed = p.speed * speedMult;

                        var dmgCreaterComp = p.GetComponent<DamageCreater>();

                        if (dmgCreaterComp)
                        {
                            dmgCreaterComp.Init(stats.damage, dealDamageTo, gameObject);
                        }

                        p.RefreshLastPos();

                        var muzzleFlashObj = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, muzzleVFXPool, shootingPoint.position, p.transform.rotation);
                        if (muzzleFlashObj)
                        {
                            var muzzleFlashScaleOriginal = muzzleFlashObj.transform.localScale;
                            muzzleFlashObj.transform.SetParent(shootingPoint);
                            muzzleFlashObj.transform.localPosition = Vector3.zero;
                            muzzleFlashObj.transform.localScale = muzzleFlashScaleOriginal;
                        }
                    }
                }
            }
        }

        public void Born()
        {
            gameObject.SetActive(true);

            AddBonusToPlayer();

            if (!m_attackRefreshed)
                attackCo = StartCoroutine(Attack());
        }

        public void Die()
        {
            m_attackRefreshed = false;
            StopCoroutine(attackCo);
            RemoveBonus();
            gameObject.SetActive(false);
        }

        public void AddBonusToPlayer()
        {
            m_player.Gun.stats.damage.AddModifier(stats.damageBonus);
            m_player.stats.health.AddModifier(stats.healthBonus);
            m_player.CurHealth = m_player.stats.health.GetValue();
        }

        public void RemoveBonus()
        {
            m_player.Gun.stats.damage.RemoveModifier(stats.damageBonus);
            m_player.stats.health.RemoveModifier(stats.healthBonus);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color32(8, 196, 215, 85);
            Gizmos.DrawSphere(transform.position, searchRange);
        }
    }
}
