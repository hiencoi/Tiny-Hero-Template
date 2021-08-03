using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UDEV.SPM;
using UDEV.AI2D;

namespace UDEV.TinyHero
{
    public class MissilesCall : Skill
    {

        [Header("Skill Settings:")]

        public MissilesCallStats stats;

        public float searchRange;

        public float missileSpeed;

        [TagList]
        public string enemyTag;

        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string missilePool;

        public DamageTo damageTo;

        List<GameObject> m_targets;

        bool m_fired;

        protected override void Init()
        {
            OnTriggerBegin.AddListener(() =>
            {
                FireMissile();
            });
        }

        public override void SetData()
        {
            Data = stats;
        }

        void FindTarget()
        {
            m_targets = new List<GameObject>();

            Collider2D[] objectsFinded = Physics2D.OverlapCircleAll(m_player.transform.position, searchRange);

            var temps = new List<GameObject>();

            var roots = new List<GameObject>();

            // Find all game objects identified by Tag
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

            // Get all root of game objects finded by Physics2D.OverlapCircleAll
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

            // Remove dupplicate roots
            if (roots != null && roots.Count > 0)
            {
                m_targets = new List<GameObject>();
                m_targets = roots.GroupBy(r => r.GetInstanceID()).Select(r => r.First()).ToList();
            }
        }

        void FireMissile()
        {
            FindTarget();

            if (m_targets != null && m_targets.Count > 0)
            {
                //If targets more than missiles player have
                if (m_targets.Count >= stats.maxMissiles)
                {
                    for (int i = 0; i < stats.maxMissiles; i++)
                    {
                        GameObject target = m_targets[Random.Range(0, m_targets.Count)];

                        if (target)
                        {
                            m_targets.Remove(target);

                            MissilesInstantiate(target);
                        }
                    }
                }
                //If targets less than missiles player have
                else
                {
                    int maxMissilesTemp = stats.maxMissiles;

                    while (maxMissilesTemp > 0)
                    {
                        int count = m_targets.Count;

                        if (maxMissilesTemp <= m_targets.Count)
                            count = maxMissilesTemp;

                        for (int i = 0; i < count; i++)
                        {
                            if (m_targets[i] != null)
                            {
                                MissilesInstantiate(m_targets[i]);
                                maxMissilesTemp--;
                            }
                        }
                    }
                }
            }
        }

        void MissilesInstantiate(GameObject target)
        {
            Vector2 firePos = GetRandomFirePos();

            var missileClone = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON ,missilePool, firePos, Quaternion.identity);

            Missile missileComp = null;

            if (missileClone)
                missileComp = missileClone.GetComponent<Missile>();

            if (missileComp)
            {
                missileComp.dealDamageTo = damageTo;
                missileComp.damage = stats.damage;
                missileComp.SetTarget(target);
                missileComp.speed = missileSpeed;
                missileComp.RefreshLastPos();
            }
        }

        Vector2 GetRandomFirePos()
        {
            float randomX = Mathf.Abs(m_player.transform.position.x) + searchRange;

            float x = Random.Range(-randomX, randomX);

            float y = m_player.transform.position.y + 10;

            return new Vector2(x, y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color32(8, 196, 215, 85);
            if(m_player)
                Gizmos.DrawSphere(m_player.transform.position, searchRange);
        }
    }
}
