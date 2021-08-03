using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UDEV.AI2D;
using UnityEngine.Events;

namespace UDEV.TinyHero
{
    public class ExplosionController : DamageCreater
    {
        [Header("Base Settings:")]
        public float damageRadius;

        List<RaycastHit2D> m_hits;

        public List<RaycastHit2D> Hits { get => m_hits;}

        float m_originalDmg;

        public override void DealDamage()
        {
            m_originalDmg = damage;

            List<GameObject> temps = new List<GameObject>();
            List<GameObject> targets = new List<GameObject>();
            m_hits = new List<RaycastHit2D>();

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, damageRadius);
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] != null)
                {
                    temps.Add(cols[i].gameObject);
                }
            }

            /*
             * Find all root of game objects finded by Physics2D.OverlapCircleAll
             */
            if (temps != null && temps.Count > 0)
            {
                foreach (GameObject target in temps)
                {
                    if (target != null)
                    {
                        var root = target.transform.root;

                        if (root) targets.Add(root.gameObject);
                    }
                }
            }

            if (targets != null && targets.Count > 0)
            {
                //Remove dupplicate roots
                targets = targets.GroupBy(r => r.GetInstanceID()).Select(r => r.First()).ToList();

                foreach (GameObject target in targets)
                {
                    Vector2 grPos = new Vector2(transform.position.x, transform.position.y);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(grPos, new Vector2(target.transform.position.x, target.transform.position.y) - grPos, damageRadius);
                    for (int h = 0; h < hits.Length; h++)
                    {
                        if (hits[h].collider.transform.root.gameObject == target)
                        {
                            m_hits.Add(hits[h]);
                            // Calculate the damage based on distance:
                            float finalDamage = damage * (1 - ((new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(hits[h].point.x, hits[h].point.y)).magnitude / damageRadius));
                            finalDamage = Mathf.Abs(finalDamage);
                            // Apply damage:

                            damage = finalDamage;

                            ApplyDamage(hits[h].collider.gameObject);

                            var dmgCreaterComp = target.GetComponent<DamageCreater>();

                            if (dmgCreaterComp)
                            {
                                if (dmgCreaterComp.explosible && !dmgCreaterComp.DamageDealed)
                                {
                                    dmgCreaterComp.DealDamageWhenAffected();
                                }
                            }

                            damage = m_originalDmg;
                        }
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, damageRadius);
        }
    }
}
