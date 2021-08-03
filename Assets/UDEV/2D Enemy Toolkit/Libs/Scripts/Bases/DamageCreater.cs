using UnityEngine;
using System;
using UDEV.SPM;

namespace UDEV.AI2D
{
    public enum DamageTo
    {
        PLAYER,
        AI,
        ALL,
        NONE
    }

    /// <summary>
    /// Base class for objects can deal damage to others game objects
    /// </summary>
    public class DamageCreater : MonoBehaviour
    {
        [Header("Damage Creater Settings:")]
        public float damage;
        public bool explosible;
        private bool m_damageDealed;
        GameObject m_objectTriggered;
        bool m_isHolding;

        [TagList]
        public string playerTag;
        [TagList]
        public string enemyTag;
        [TagList]
        public string obstacleTag;
        public DamageTo dealDamageTo;

        public Action OnHit;
        public Action OnObstacleHit;

        public bool DamageDealed { get => m_damageDealed; set => m_damageDealed = value; }
        public GameObject ObjectTriggered { get => m_objectTriggered;}
        public bool IsHolding { get => m_isHolding; set => m_isHolding = value; }

        public void Init(float _dmg, DamageTo _damageTo, GameObject _objTriggerd = null, bool _isHolding = false)
        {
            damage = _dmg;
            dealDamageTo = _damageTo;
            m_objectTriggered = _objTriggerd;
            m_isHolding = _isHolding;
        }

        public virtual void DealDamage()
        {

        }

        public void ApplyDamage(GameObject damagedTarget)
        {
            if (damagedTarget && ObjectTriggered && ObjectTriggered.transform != damagedTarget.transform && damagedTarget.transform != transform)
            {
                if (DealCondition(damagedTarget))
                {
                    DamageTaker damageTakerComp = damagedTarget.GetComponent<DamageTaker>();

                    if (damageTakerComp != null)
                    {
                        damageTakerComp.TakeDamage(damage);
                    }

                    if (OnHit != null)
                        OnHit.Invoke();
                }
                else if (damagedTarget.CompareTag(obstacleTag))
                {
                    if (OnObstacleHit != null)
                        OnObstacleHit.Invoke();
                }
            }
        }

        public virtual void DealDamageWhenAffected()
        {

        }

        public bool DealCondition(GameObject col)
        {
            bool cond = false;
            switch (dealDamageTo)
            {
                case DamageTo.AI:
                    cond = col.CompareTag(enemyTag);
                    break;
                case DamageTo.PLAYER:
                    cond = col.CompareTag(playerTag);
                    break;
                case DamageTo.ALL:
                    cond = col.CompareTag(enemyTag) || col.CompareTag(playerTag);
                    break;
            }
            return cond;
        }
    }
}
