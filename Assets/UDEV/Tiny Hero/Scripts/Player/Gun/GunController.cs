using UnityEngine;
using UnityEngine.Events;
using UDEV.SPM;
using UDEV.AI2D;

namespace UDEV.TinyHero
{
    public class GunController : DamageCreater
    {
        [Header("Main Settings:")]
        [UniqueId]
        public string id;
        public Facing facing;
        public new string name;
        public Sprite hudIcon;
        public Transform shootingPoint;
        public GunStats stats;
        public int numberOfProjectilesPerShot = 1;
        public float spread;
        public float rateOfFire;

        [Header("Pooler Settings:")]
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string projectilePool;
        [PoolerKeys(target = PoolerTarget.WEAPON)]
        public string muzzleFlashPool;

        [Header("Kickback Visual Settings:")]
        [Range(0.1f, 1f)]
        public float kickBackForce = 0.15f;
        [Range(10f, 100f)]
        public float kickBackForceMulti = 10f;
        Vector3 m_startPos;

        [Header("Events: ")]
        public UnityEvent OnInit;
        public UnityEvent InShooting;
        public UnityEvent InReloading;
        public UnityEvent ReloadFinish;

        Player m_player;
        int m_curAmmo;
        float m_speedMult;
        float m_curFR;
        float m_curReloadTime;
        [HideInInspector]
        public Vector3 shootingDirection;
        bool m_isReloading;
        bool m_canShoot;

        public int CurAmmo { get => m_curAmmo;}
        public float CurReloadTime { get => m_curReloadTime;}
        public Player Player { get => m_player; set => m_player = value; }
        public bool IsReloading { get => m_isReloading;}
        public bool CanShoot { get => m_canShoot; set => m_canShoot = value; }

        private void Awake()
        {
            stats.Init();
            LoadData();
            m_curFR = 1;
        }

        // Use this for initialization
        void Start()
        {
            Init();
        }

        void Init()
        {
            m_startPos = m_player.gunPoint.localPosition;

            FlipFollowPlayer();

            if (OnInit != null)
                OnInit.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            ReloadListener();
            RateOfFireCountDown();
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_startPos, kickBackForce * kickBackForceMulti * Time.deltaTime);
        }

        void RateOfFireCountDown()
        {
            if (m_curAmmo <= 0)
                m_isReloading = true;

            if (m_curAmmo > 0 && !m_isReloading)
            {
                if (m_curFR < 1)
                    m_curFR += Time.deltaTime * rateOfFire;
            }
            else
            {
                m_curAmmo = 0;
            }
        }

        public override void DealDamage()
        {
            if (m_curFR >= 1 && !m_isReloading)
            {
                m_curFR = 0;

                m_curAmmo -= 1;

                // Spawn projectile:
                for (int i = 0; i < numberOfProjectilesPerShot; i++)
                {
                    GameObject p = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON ,projectilePool, shootingPoint.position, transform.rotation);

                    if (p)
                    {
                        Projectile pComp = p.GetComponent<Projectile>();

                        if (pComp)
                        {
                            var dmgCreateComp = pComp.GetComponent<DamageCreater>();

                            if (dmgCreateComp)
                            {
                                dmgCreateComp.Init(stats.damage.GetValue(), dealDamageTo, gameObject);
                            }

                            FlipProjectileFollowWeapon(pComp);

                            // Spread:
                            p.transform.localEulerAngles = p.transform.localEulerAngles + new Vector3(0, 0, -spread + ((i + 0.5f) * ((spread * 2) / numberOfProjectilesPerShot)));

                            pComp.curSpeed = pComp.speed * m_speedMult;

                            pComp.RefreshLastPos();

                            if (InShooting != null)
                                InShooting.Invoke();
                        }
                    }
                }

                // Kick back:
                shootingDirection.Normalize();
                Kickback(shootingDirection);

                // Muzzle flash VFX:
                var muzzleFlashObj = PoolersManager.Ins.Spawn(PoolerTarget.WEAPON ,muzzleFlashPool, shootingPoint.position, transform.rotation);
                if (muzzleFlashObj)
                {
                    var muzzleFlashScaleOriginal = muzzleFlashObj.transform.localScale;
                    muzzleFlashObj.transform.SetParent(shootingPoint);
                    muzzleFlashObj.transform.localPosition = Vector3.zero;
                    muzzleFlashObj.transform.localScale = muzzleFlashScaleOriginal;
                }
            }
        }

        void Kickback(Vector3 shootingDirection)
        {
            switch (m_player.facing)
            {
                case Facing.LEFT:
                    if (shootingDirection.x < 0)
                    {
                        var kickBackDir = new Vector3(-shootingDirection.x, -shootingDirection.y, shootingDirection.z);
                        transform.localPosition += kickBackDir * kickBackForce;
                    }
                    else if (shootingDirection.x > 0)
                    {
                        var kickBackDir = new Vector3(shootingDirection.x, -shootingDirection.y, shootingDirection.z);
                        transform.localPosition += kickBackDir * kickBackForce;
                    }
                    break;
                case Facing.RIGHT:
                    if (shootingDirection.x < 0)
                    {
                        var kickBackDir = new Vector3(shootingDirection.x, -shootingDirection.y, shootingDirection.z);
                        transform.localPosition += kickBackDir * kickBackForce;
                    }
                    else if (shootingDirection.x > 0)
                    {
                        var kickBackDir = new Vector3(-shootingDirection.x, -shootingDirection.y, shootingDirection.z);
                        transform.localPosition += kickBackDir * kickBackForce;
                    }

                    break;
            }
        }

        void ReloadListener()
        {
            if (m_isReloading)
            {
                m_curReloadTime += Time.deltaTime;

                if (InReloading != null)
                    InReloading.Invoke();

                if (m_curReloadTime >= stats.reloadTime.GetValue())
                {
                    m_curAmmo = stats.ammo.GetIntValue();

                    m_curReloadTime = 0;

                    m_isReloading = false;

                    if (ReloadFinish != null)
                        ReloadFinish.Invoke();
                }
            }
        }

        public void ReloadImmediately()
        {
            m_curAmmo = stats.ammo.GetIntValue();
            m_isReloading = false;

            if (ReloadFinish != null)
                ReloadFinish.Invoke();
        }

        public void Reload()
        {
            if (m_isReloading || m_curAmmo >= stats.ammo.GetIntValue()) return;

            m_curReloadTime = 0;

            m_isReloading = true;
        }

        void FlipFollowPlayer()
        {
            if (!m_player) return;

            switch (m_player.facing)
            {
                case Facing.LEFT:

                    if (facing == Facing.RIGHT)
                        FlipLocalScaleX();

                    break;
                case Facing.RIGHT:

                    if (facing == Facing.LEFT)
                        FlipLocalScaleX();

                    break;
            }
        }

        void FlipLocalScaleX()
        {
            transform.localScale = new Vector3(
                            transform.localScale.x * -1,
                            transform.localScale.y,
                            transform.localScale.z);
        }

        void FlipProjectileFollowWeapon(Projectile p)
        {
            if (!m_player) return;

            switch (m_player.facing)
            {
                case Facing.LEFT:
                    FlipProjectileFollowWeaponLeft(p);
                    break;
                case Facing.RIGHT:
                    FlipProjectileFollowWeaponRight(p);
                    break;
            }
        }

        void FlipProjectileFollowWeaponRight(Projectile p)
        {
            Vector3 pScale = p.transform.localScale;

            m_speedMult = 1;

            if (shootingDirection.x < 0)
            {
                pScale.x *= -1;

                m_speedMult = -1;

                if (p.transform.localScale.x > 0)
                    p.transform.localScale = pScale;
            }
            else if (shootingDirection.x > 0)
            {
                if (p.transform.localScale.x < 0)
                    pScale.x *= -1;

                p.transform.localScale = pScale;
            }
        }

        void FlipProjectileFollowWeaponLeft(Projectile p)
        {
            Vector3 pScale = p.transform.localScale;

            m_speedMult = 1;

            if (shootingDirection.x < 0)
            {
                if (p.transform.localScale.x < 0)
                    pScale.x *= -1;

                p.transform.localScale = pScale;
            }
            else if (shootingDirection.x > 0)
            {
                pScale.x *= -1;

                m_speedMult = -1;

                if (p.transform.localScale.x > 0)
                    p.transform.localScale = pScale;
            }
        }

        public void Upgrade()
        {
            stats.Upgrade(id);
            LoadData();
            
        }

        void LoadData()
        {
            stats.LoadData(id);

            m_curAmmo = stats.ammo.GetIntValue();
        }
    }
}
