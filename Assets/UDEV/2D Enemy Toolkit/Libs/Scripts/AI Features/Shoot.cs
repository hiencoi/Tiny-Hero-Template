using UnityEngine;
using UDEV.SPM;

namespace UDEV.AI2D
{
    public class Shoot : aiFeature
    {
        [Header("Feature Settings:")]
        public float distCondition; // Distance feature can be trigger
        public float minDistCondition; //Min Distance feature can be trigger
        public DamageTo damageTo;
        public int numberOfProjectilesPerShot = 1;
        public float spread;
        public float rateOfFire;
        float curFR;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string projectilePool;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string muzzleFlashPool;
        public Transform[] shootingPoints;
        public AudioClip[] shootingSounds;
        public bool TriggerOneDirection;
        Vector3 shootingDirection;
        bool m_isHaveDirection;

        protected override void Init()
        {
            curFR = 1f;
        }

        protected override bool TriggerCondition()
        {
            return m_aiController.DistToPlayer <= distCondition && m_aiController.DistToPlayer > minDistCondition;
        }

        protected override void Core()
        {
            ShootTrigger();
        }

        void ShootTrigger()
        {
            if (curFR < 1)
                curFR += Time.deltaTime * rateOfFire;

            if (curFR >= 1)
            {
                curFR = 0;
                foreach (Transform shootingPoint in shootingPoints)
                {
                    ShootMain(shootingPoint);
                }
            }
        }

        void ShootMain(Transform shootingPoint)
        {
            Projectile p = null;

            // Sound:
            AudioController.Ins.PlaySound(shootingSounds);

            if (TriggerOneDirection)
            {
                if (!m_isHaveDirection)
                {
                    shootingDirection = m_aiController.Player.transform.position - shootingPoint.position;
                    shootingDirection.Normalize();
                    m_isHaveDirection = true;
                }
            }
            else
            {
                shootingDirection = m_aiController.Player.transform.position - shootingPoint.position;
                shootingDirection.Normalize();
            }

            // Spawn projectile:

            var rotZValue = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;

            for (int i = 0; i < numberOfProjectilesPerShot; i++)
            {
                var projectile = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON, projectilePool, shootingPoint.position, Quaternion.identity);
                if (projectile)
                {
                    p = projectile.GetComponent<Projectile>();

                    if (p == null) return;

                    p.dealDamageTo = damageTo;

                    p.transform.rotation = Quaternion.Euler(0f, 0f, rotZValue);
                    Vector3 pScale = p.transform.localScale;

                    float speedMult = 1;

                    // Spread:
                    p.transform.localEulerAngles = p.transform.localEulerAngles + new Vector3(0, 0, -spread + ((i + 0.5f) * ((spread * 2) / numberOfProjectilesPerShot)));

                    p.curSpeed = p.speed * speedMult;
                    
                    p.RefreshLastPos();

                    var dmgCreaterCmp = p.GetComponent<DamageCreater>();

                    if (dmgCreaterCmp)
                    {
                        dmgCreaterCmp.Init(damage.GetValue(), damageTo, m_aiController.gameObject);
                    }
                }
                
            }

            // Muzzle flash VFX:
            var muzzleFlashObj = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON ,muzzleFlashPool, shootingPoint.position, p.transform.rotation);
            if (muzzleFlashObj)
            {
                muzzleFlashObj.transform.SetParent(shootingPoint);
                muzzleFlashObj.transform.localPosition = Vector3.zero;
            }
        }

        protected override void FinalReset()
        {
            m_isHaveDirection = false;
        }
    }
}
