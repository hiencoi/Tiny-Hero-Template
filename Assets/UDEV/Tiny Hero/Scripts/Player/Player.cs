using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.AI2D;
using MonsterLove.StateMachine;
using UnityEngine.Events;

namespace UDEV.TinyHero
{
    public enum Facing
    {
        NONE,
        LEFT,
        RIGHT
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : DamageCreater, DamageTaker
    {
        [UniqueId]
        public string id;

        //States for behavior
        public enum States
        {
            Idle,
            Moving,
            Death,
            SkillActive
        }

        private StateMachine<States> m_fsm; // FSM variable

        [Header("Base Settings:")]
        public PlayerStats stats;
        public Facing facing;
        public float moveSpeed;
        public float jumpForce;

        [Header("Check On Ground Settings:")]
        [LayerList]
        public int groundLayer; //Ground layer
        public float groundCheckScaleX = 0.5f; //Scale x to dection slope
        public float groundCheckScaleY = 0.5f; //Scale y to dection slope
        public float groundCheckDistance = 1f; //Distance to check ai on ground or not
        public Vector3 groundCheckOffset;

        [Header("Smooth Jump Settings:")]
        [Range(0f, 5f)]
        public float fallMultipiler = 2.5f;
        [Range(0f, 5f)]
        public float lowJumpMultipiler = 2.5f;

        [Header("Invisible Settings:")]
        [LayerList]
        public int normalLayer;
        [LayerList]
        public int invincibleLayer;
        public float invincibleTime;

        [Header("Skill Settings:")]
        public SkillsManager skillsManager;
        public int maxSkill;

        bool m_isGrounded;
        bool m_lastFrameGrounded;
        float m_lastGroundedTime;

        [Header("Weapon Settings:")]
        public Transform gunPoint;
        public Vector2 gunPointOffset;
        GunController m_gun;

        [Header("Animation Settings:")]
        public Animator anim;
        [AnimatorStates]
        public AnimState idleState;
        [AnimatorStates]
        public AnimState movingState;

        [Header("Events:")]
        public UnityEvent OnInit;
        public UnityEvent OnJump;
        public UnityEvent OnLand;
        public UnityEvent OnTakeDamage;
        public UnityEvent OnLostLife;
        public UnityEvent OnDead;
        public UnityEvent OnRevive;
        public UnityEvent OnLevelup;
        public UnityEvent OnUpdateGun;

        float m_curHealth;
        float m_curInvincibleTime;
        bool m_isDead;
        bool m_isCanMove = true;
        bool m_isCanJump = true;
        bool m_isCanShoot = true;
        bool m_isInvincible;
        Rigidbody2D m_rb;
        float m_xLimitPos;

        public GunController Gun { get => m_gun; set => m_gun = value; }
        public bool IsDead { get => m_isDead;}
        public float CurHealth { get => m_curHealth; set => m_curHealth = Mathf.Clamp(value, 0, stats.health.GetValue()); }
        public bool IsGrounded { get => m_isGrounded;}
        public float CurInvincibleTime { get => m_curInvincibleTime; set => m_curInvincibleTime = value; }
        public StateMachine<States> Fsm { get => m_fsm; set => m_fsm = value; }
        public bool IsCanMove { get => m_isCanMove; set => m_isCanMove = value; }
        public bool IsCanJump { get => m_isCanJump; set => m_isCanJump = value; }
        public bool IsInvincible { get => m_isInvincible;}
        public bool IsCanShoot { get => m_isCanShoot; set => m_isCanShoot = value; }

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        void Init()
        {
            stats.Init();

            LoadData();

            UpdateGun(GunShopData.Ins.curGun);

            m_fsm = StateMachine<States>.Initialize(this);

            m_fsm.ChangeState(States.Idle);

            m_xLimitPos = GameManager.Ins.cam.MapLimitX;

            if (skillsManager)
                skillsManager.InitializeSkills(this);

            if (OnInit != null)
                OnInit.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            DealDamage();
        }

        private void FixedUpdate()
        {
            CheckOnGround();

            if (GamePadsController.Ins.IsJumpBtnHolding)
            {
                Jump();
            }

            SmoothJump();
        }

        public override void DealDamage()
        {
            if (!m_gun || !m_isCanShoot) return;

            m_gun.CanShoot = GamePadsController.Ins.IsShootingBtnPressed;

            if (GamePadsController.Ins.IsShootingBtnPressed)
            {
                if (m_gun)
                {
                    if (GamePadsController.Ins.isOnMobile)
                    {
                        m_gun.shootingDirection = GamePadsController.Ins.ShootDir;
                        Flip();
                        m_gun.DealDamage();
                    }
                    else
                    {
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        Vector3 shootingDir = mousePos - m_gun.shootingPoint.position;

                        m_gun.shootingDirection = shootingDir;

                        GamePadsController.Ins.ShootDir = shootingDir;

                        if (Vector3.Magnitude(shootingDir) >= 10.3f)
                        {
                            Flip();
                        }
                    }
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (m_isDead || m_isInvincible) return;

            Invincible(m_curInvincibleTime);

            if (m_curHealth > 0)
            {
                m_curHealth -= damage;

                if (m_curHealth <= 0)
                {
                    GameManager.Ins.CurLife--;
                    if (OnLostLife != null)
                        OnLostLife.Invoke();
                    SaveData();
                    LoadData();
                }

                if (OnTakeDamage != null)
                    OnTakeDamage.Invoke();
            } 
        }

        public void Invincible(float time)
        {
            m_isInvincible = true;

            gameObject.layer = invincibleLayer;

            Timer.Schedule(this, time, () => {
                BackToNormal();
            });
        }

        void BackToNormal()
        {
            m_isInvincible = false;

            gameObject.layer = normalLayer;
        }

        public void Revive()
        {
            gameObject.SetActive(true);

            m_isDead = false;

            LoadData();

            Invincible(m_curInvincibleTime);

            m_gun.ReloadImmediately();

            if (skillsManager)
                skillsManager.Recover();

            if (OnRevive != null)
                OnRevive.Invoke();
        }

        public void Die()
        {
            m_isDead = true;

            m_curHealth = 0;

            if (OnDead != null)
                OnDead.Invoke();

            gameObject.SetActive(false);
        }

        public void AddXp(int xpBonus)
        {
            stats.xp += xpBonus;

            SaveData();

            if (stats.CanLevelUp() && !stats.IsMaxLevel())
            {
                Invincible(m_curInvincibleTime);

                stats.LevelUp();

                SaveData();

                LoadData();

                if (OnLevelup != null)
                    OnLevelup.Invoke();
            }
        }

        public void UpdateGun(GunController newGun)
        {
            if (m_gun)
            {
                m_gun.gameObject.SetActive(false);
            }

            m_gun = newGun;
            m_gun.dealDamageTo = dealDamageTo;
            m_gun.gameObject.SetActive(true);
            GunVisualHandle gunVisualHandle = m_gun.GetComponent<GunVisualHandle>();
            if (gunVisualHandle)
                gunVisualHandle.PlayEquippedSounds();

            if (OnUpdateGun != null)
                OnUpdateGun.Invoke();
        }

        public void MoveLeft()
        {
            if (!m_isCanMove) return;

            if (m_rb)
                m_rb.velocity = new Vector2(-moveSpeed, m_rb.velocity.y);

            Vector3 playerPosLimited = new Vector3(
                Mathf.Clamp(transform.position.x, -m_xLimitPos, m_xLimitPos),
                transform.position.y, transform.position.z);

            transform.position = playerPosLimited;
        }

        public void MoveRight()
        {
            if (!m_isCanMove) return;
            if (m_rb)
                m_rb.velocity = new Vector2(moveSpeed, m_rb.velocity.y);

            Vector3 playerPosLimited = new Vector3(
                Mathf.Clamp(transform.position.x, -m_xLimitPos, m_xLimitPos),
                transform.position.y, transform.position.z);

            transform.position = playerPosLimited;
        }

        public void Jump()
        {
            if (m_isGrounded && m_rb && m_isCanJump)
            {
                m_isGrounded = false;

                m_rb.velocity = new Vector2(m_rb.velocity.x, jumpForce);

                if (OnJump != null)
                    OnJump.Invoke();
            }
        }

        void SmoothJump()
        {
            if (!m_isGrounded)
            {
                if (m_rb.velocity.y < 0)
                {
                    m_rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultipiler - 1) * Time.deltaTime;
                }
                else if (m_rb.velocity.y > 0 && !GamePadsController.Ins.IsJumpBtnHolding)
                {
                    m_rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultipiler - 1) * Time.deltaTime;
                }
            }
        }

        void CheckOnGround()
        {
            m_isGrounded = false;
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + groundCheckOffset, new Vector2(groundCheckScaleX, groundCheckScaleY), 0, Vector2.down, groundCheckDistance, 1 << groundLayer);
            if (hit.collider != null)
            {
                m_isGrounded = true;

                if (hit.normal.y != 0)
                {
                    var slope = -hit.normal.x / hit.normal.y;

                    var rotZ = slope * Mathf.Rad2Deg;

                    transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y,
                        rotZ);

                    if (m_rb.velocity.y > 0)
                        m_rb.velocity = new Vector2(m_rb.velocity.x, m_rb.velocity.y * -1f);
                }
            }
            else
            {
                m_isGrounded = false;
            }

            if (m_isGrounded)
            {
                if (!m_lastFrameGrounded && (Time.time - m_lastGroundedTime) > 0.1f)
                    Land();
            }
            else
            {
                m_lastFrameGrounded = m_isGrounded;
            }
        }

        void Land()
        {
            m_lastFrameGrounded = m_isGrounded;
            m_lastGroundedTime = Time.time;

            if (OnLand != null)
                OnLand.Invoke();
        }

        void MoveListener()
        {
            if (GamePadsController.Ins.IsMLeftBtnPressed)
            {
                MoveLeft();
            }
            else if (GamePadsController.Ins.IsMRightBtnPressed)
            {
                MoveRight();
            }
            else if(m_fsm.State != States.SkillActive)
            {
                m_rb.velocity = new Vector2(0f, m_rb.velocity.y);

                m_fsm.ChangeState(States.Idle);
            }
        }

        public void LoadData()
        {
            string json = CPlayerPrefs.GetString(GameConsts.PLAYER_DATA + id, null);

            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, stats);
            }

            m_curHealth = stats.health.GetValue();

            m_curInvincibleTime = invincibleTime;
        }

        public void SaveData()
        {
            string json = JsonUtility.ToJson(stats);
            CPlayerPrefs.SetString(GameConsts.PLAYER_DATA + id, json);
        }

        #region FSM
        void Idle_Update()
        {
            MoveListener();

            Helper.PlayAnimatorState(anim, idleState.layerIndex, idleState.name);

            if ((GamePadsController.Ins.IsMRightBtnPressed ||
                GamePadsController.Ins.IsMLeftBtnPressed) && m_isGrounded)
                m_fsm.ChangeState(States.Moving);
        }

        void Moving_FixedUpdate()
        {
            MoveListener();

            if (!m_isGrounded)
                m_fsm.ChangeState(States.Idle);

            Helper.PlayAnimatorState(anim, movingState.layerIndex ,movingState.name);
        }

        void SkillActive_Update()
        {
            MoveListener();

            if (!GameManager.Ins.IsGameover && GameManager.Ins.IsGamebegin)
                skillsManager.TriggerSkills();
        }

        #endregion

        #region Flip Handlee
        /// <summary>
        /// Flip player follow anim direction
        /// </summary>
        void Flip()
        {
            if (m_gun && !GamePadsController.Ins.IsShootingBtnPressed) return;

            switch (facing)
            {
                case Facing.LEFT:
                    FaceLeftFlip(m_gun.shootingDirection);
                    break;
                case Facing.RIGHT:
                    FaceRightFlip(m_gun.shootingDirection);
                    break;
            }
        }

        /// <summary>
        /// Flip player when he face left
        /// </summary>
        /// <param name="dir">Direction</param>
        void FaceLeftFlip(Vector3 dir)
        {
            Helper.FaceLeftFlip(dir, transform);

            FaceLeftWeaponRotate(dir);
        }

        /// <summary>
        /// Flip player when he face right
        /// </summary>
        /// <param name="dir">Direction</param>
        void FaceRightFlip(Vector3 dir)
        {
            Helper.FaceRightFlip(dir, transform);

            FaceRightWeaponRotate(dir);
        }

        /// <summary>
        /// Rotate weapon when it face left
        /// </summary>
        /// <param name="dir">Direction</param>
        void FaceLeftWeaponRotate(Vector3 dir)
        {
            m_gun.shootingDirection = dir;
            var rotZValue = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            m_gun.transform.localRotation = Quaternion.Euler(0f, 0f, transform.localScale.x < 0 ? rotZValue * -1 : rotZValue + 180);
            m_gun.DealDamage();
        }

        /// <summary>
        /// Rotate weapon when it face right
        /// </summary>
        /// <param name="dir">Direction</param>
        void FaceRightWeaponRotate(Vector3 dir)
        {
            m_gun.shootingDirection = dir;
            var rotZValue = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            m_gun.transform.localRotation = Quaternion.Euler(0f, 0f, transform.localScale.x < 0 ? 180 - rotZValue : rotZValue);
            m_gun.DealDamage();
        }
        #endregion

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(enemyTag))
            {
                aiBase aiBaseComp = col.gameObject.GetComponent<aiBase>();

                if (aiBaseComp)
                    TakeDamage(aiBaseComp.damage);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag(enemyTag))
            {
                aiBase aiBaseComp = col.GetComponent<aiBase>();

                if (aiBaseComp && !aiBaseComp.IsDead)
                    TakeDamage(aiBaseComp.damage);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + groundCheckOffset, new Vector3(transform.position.x + groundCheckOffset.x,
                transform.position.y + groundCheckOffset.y + -groundCheckDistance,
                transform.position.z));
        }
    }
}
