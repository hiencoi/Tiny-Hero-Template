using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MonsterLove.StateMachine;
using UDEV.TinyHero;

namespace UDEV.AI2D
{
    public class aiBase : DamageCreater, DamageTaker
    {
        //Sprite facing
        public enum Facing
        {
            NONE, LEFT, RIGHT
        }

        //States for behavior
        public enum States
        {
            Idle,
            Patrol,
            Chase,
            Action,
            TakeDamage,
            Death
        }

        protected StateMachine<States> m_fsm; // FSM variable

        [Header("Base Settings:")]
        public Facing facing; //Sprite facing
        public float searchRange; //Radius of search range
        public float maxMoveDistance; //Max distance ai can move
        public bool alwaysFollow; //Ai always chase to player or not
        [Header("Stats Settings:")]
        public Stat health;
        public Stat bodyDamage;
        public Stat patrolSpeed;
        public Stat chaseSpeed;
        public Stat knockbackForce;
        public Stat knockbackTime;
        public Stat invincibleTime;

        [Header("Animator States:")]
        public Animator anim; //Animator controller
        [AnimatorStates]
        public AnimState idleState;
        [AnimatorStates]
        public AnimState patrolState; // Patrol Animator State name
        [AnimatorStates]
        public AnimState chaseState; // Chase Animator State name
        [AnimatorStates]
        public AnimState takeDmgState;
        [AnimatorStates]
        public AnimState deathState; // Death State Animator State name

        [Header("Others Settings:")]
        public aiFeaturesManager featuresManager; //Manager of features
        private aiFeature m_activeFeature; //Current feature ai using
        [LayerList]
        public int deathLayer;
        [LayerList]
        public int normalLayer;

        GameObject m_player; // Current Player
        Vector3 m_playerDir;
        Vector3 m_facingDir;
        float m_distToPlayer;
        float m_playerAngle;

        private float m_curHealth; // Current ai health
        float m_invincibleTime;
        float m_knockbackTime;
        float m_curSpeed; // Current ai speed


        [Header("Events:")]
        public UnityEvent OnInit;
        public UnityEvent OnDead; // Event manager when ai die
        public UnityEvent OnTakeDamage; // Event manager when ai take damage

        bool m_isFreeze;
        private bool isDead;
        protected bool m_isInvincibled;
        protected bool m_canKnockback;
        protected bool m_idleChecked;
        protected bool m_playerDetected;

        List<DamageCreater> m_weapons;

        [HideInInspector]
        public Rigidbody2D rb; // Rigibody2d component


        #region Getter_Setter
        public float CurHealth { get => m_curHealth; set => m_curHealth = Mathf.Clamp(value, 0, health.GetValue()); }
        public GameObject Player { get => m_player; }
        public float CurSpeed { get => m_curSpeed; set => m_curSpeed = value; }
        public Vector3 PlayerDir { get => m_playerDir; set => m_playerDir = value; }
        public float DistToPlayer { get => m_distToPlayer; }
        public bool IsFreeze { get => m_isFreeze; set => m_isFreeze = value; }
        public List<DamageCreater> Weapons { get => m_weapons; set => m_weapons = value; }
        public aiFeature ActiveFeature { get => m_activeFeature; set => m_activeFeature = value; }
        public bool IsDead { get => isDead; set => isDead = value; }
        public float PlayerAngle { get => m_playerAngle; set => m_playerAngle = value; }

        #endregion

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Init();
        }

        public virtual void OnEnable()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            //Get direction to player and distance to player
            if (m_player)
            {
                m_distToPlayer = Vector2.Distance(transform.position, m_player.transform.position);
                m_playerDir = m_player.transform.position - transform.position;
                m_playerDir.Normalize();
                m_playerAngle = Mathf.Atan2(Mathf.Abs(m_playerDir.y), Mathf.Abs(m_playerDir.x)) * Mathf.Rad2Deg;
            }

            if (m_fsm.State != States.Death)
            {
                if (m_isInvincibled)
                {
                    m_invincibleTime -= Time.deltaTime;

                    if (m_invincibleTime <= 0)
                    {
                        m_invincibleTime = invincibleTime.GetValue();

                        m_isInvincibled = false;
                    }
                }

                KnockBack();
            }

            DetectPlayer();
        }

        /// <summary>
        /// Method for Initialize all of ai data
        /// </summary>
        protected virtual void Init()
        {
            m_weapons = new List<DamageCreater>();

            gameObject.layer = normalLayer;

            //Initialize data
            LoadData();

            //Initalize ai features
            if (featuresManager)
                featuresManager.InitializeFeatures(this);

            // If always follow option selected find player game object on scene
            if (alwaysFollow)
            {
                m_playerDetected = true;
                m_player = GameManager.Ins.Player.gameObject;
            }
            //m_player = GameObject.FindGameObjectWithTag(Constants.PlayerTag);

            if (OnInit != null)
                OnInit.Invoke();
        }

        public void LoadData()
        {
            m_curHealth = health.GetValue();
            damage = bodyDamage.GetValue();
            m_invincibleTime = invincibleTime.GetValue();
            m_knockbackTime = knockbackTime.GetValue();
        }

        public void RemoveModifiers()
        {
            health.RemoveModifiers();
            bodyDamage.RemoveModifiers();
            patrolSpeed.RemoveModifiers();
            chaseSpeed.RemoveModifiers();
            knockbackForce.RemoveModifiers();
            invincibleTime.RemoveModifiers();
        }

        /// <summary>
        /// Method for dectect player
        /// </summary>
        void DetectPlayer()
        {
            if (m_playerDetected) return;

            Collider2D[] findeds = Physics2D.OverlapCircleAll(transform.position, searchRange);

            for (int i = 0; i < findeds.Length; i++)
            {
                if (findeds[i].CompareTag(playerTag))
                {
                    m_player = findeds[i].gameObject;
                    m_playerDetected = true;
                    break;
                }
            }
        }

        public void FSM_StatesSwitch()
        {
            if (m_fsm.State != States.Death)
            {
                if (m_playerDetected)
                {
                    if (m_activeFeature != null)
                    {
                        m_fsm.ChangeState(States.Action);
                    }
                    else
                    {
                        m_fsm.ChangeState(States.Chase);
                    }
                }
                else
                {
                    m_fsm.ChangeState(States.Patrol);
                }
            }
        }

        /// <summary>
        /// Flip sprite follow a direction
        /// </summary>
        public void Flip(Vector3 dir)
        {
            m_facingDir = dir;

            switch (facing)
            {
                case Facing.LEFT:
                    Helper.FaceLeftFlip(dir, transform);
                    break;
                case Facing.RIGHT:
                    Helper.FaceRightFlip(dir, transform);
                    break;
            }

        }

        public bool IsFacingToPlayer()
        {
            bool isFacing = false;

            switch (facing)
            {
                case Facing.LEFT:
                    if (m_playerDir.x > 0)
                    {
                        if (transform.localScale.x < 0) isFacing = true;
                    }
                    else if (m_playerDir.x < 0)
                    {
                        if (transform.localScale.x > 0) isFacing = true;
                    }
                    break;
                case Facing.RIGHT:
                    if (m_playerDir.x > 0)
                    {
                        if (transform.localScale.x > 0) isFacing = true;
                    }
                    else if (m_playerDir.x < 0)
                    {
                        if (transform.localScale.x < 0) isFacing = true;
                    }
                    break;
            }

            return isFacing;
        }

        /// <summary>
        /// Take damage method
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (m_isInvincibled) return;

            m_canKnockback = true;

            m_curHealth -= damage;

            if (m_curHealth <= 0)
            {
                m_fsm.ChangeState(States.Death);
            }
            else if (m_fsm.State != States.Action)
            {
                m_fsm.ChangeState(States.TakeDamage);
            }
        }

        IEnumerator TakeDamageDelay()
        {
            if (OnTakeDamage != null)
                OnTakeDamage.Invoke();

            float delay = 0;

            var state = Helper.GetClip(anim, takeDmgState.clipName);

            if (state)
                delay = state.length;

            Helper.PlayAnimatorState(anim, takeDmgState.layerIndex, takeDmgState.name);

            yield return new WaitForSeconds(delay);

            m_isInvincibled = true;

            FSM_StatesSwitch();

            yield return null;
        }

        void KnockBack()
        {
            if (m_canKnockback)
            {
                Vector2 knockbackDir = m_playerDir.x > 0 ?
                Vector3.left : Vector3.right;

                m_knockbackTime -= Time.deltaTime;

                if (rb)
                    rb.velocity = knockbackDir * knockbackForce.GetValue();

                if (m_knockbackTime <= 0)
                {
                    m_knockbackTime = knockbackTime.GetValue();

                    m_canKnockback = false;
                }
            }
        }

        public void Invincible(float time)
        {
            m_invincibleTime = time;

            m_isInvincibled = true;
        }

        /// <summary>
        /// Coroutine when ai die
        /// </summary>

        IEnumerator Die()
        {
            IsDead = true;

            gameObject.layer = deathLayer;

            float delay = 0;

            //Play death animation if we have, 
            //if we not have ai will die immediately and hide on scene
            if (Helper.IsAnimatorCanPlayState(anim, 0, deathState.name))
            {
                var deathClip = Helper.GetClip(anim, deathState.clipName);

                if (deathClip)
                    delay = deathClip.length;

                Helper.PlayAnimatorState(anim, deathState.layerIndex, deathState.name);
            }

            if (OnDead != null)
            {
                OnDead.Invoke();
            }

            yield return new WaitForSeconds(delay);

            transform.position = new Vector3(1000f, 1000f, 0f);

            RemoveModifiers();

            if (featuresManager)
                featuresManager.RemoveAllModifiers();

            float idleDelay = 0;

            var idleClip = Helper.GetClip(anim, idleState.clipName);

            if (idleClip)
                idleDelay = idleClip.length;

            Helper.PlayAnimatorState(anim, idleState.layerIndex, idleState.name);

            yield return new WaitForSeconds(idleDelay);

            m_playerDetected = false;

            m_isFreeze = false;

            if (featuresManager)
                featuresManager.enabled = true;

            OnTakeDamage.RemoveAllListeners();

            OnInit.RemoveAllListeners();

            OnDead.RemoveAllListeners();

            gameObject.SetActive(false);
        }

        public void WeaponsInit(List<DamageCreater> weapons, float dmg, DamageTo damageTo = DamageTo.NONE, bool isHolding = false)
        {
            if (weapons != null && weapons.Count > 0)
            {
                foreach (DamageCreater weapon in weapons)
                {
                    if (weapon)
                        weapon.Init(dmg, damageTo, gameObject, isHolding);
                }
            }
        }

        public void WeaponsInit(DamageCreater weapon, float dmg, DamageTo damageTo = DamageTo.NONE, bool isHolding = false)
        {
            if (weapon)
                weapon.Init(dmg, damageTo, gameObject, isHolding);
        }

        public void WeaponsInit(float dmg, DamageTo damageTo = DamageTo.NONE, bool isHolding = false)
        {
            if (m_weapons != null && m_weapons.Count > 0)
            {
                foreach (DamageCreater weapon in m_weapons)
                {
                    if (weapon)
                        weapon.Init(dmg, damageTo, gameObject, isHolding);
                }
            }
        }

        public void WeaponsTrigger()
        {
            if (m_weapons != null && m_weapons.Count > 0)
            {
                foreach (DamageCreater weapon in m_weapons)
                {
                    if (weapon)
                        weapon.DealDamage();
                }
            }
        }

        public void WeaponsTrigger(List<DamageCreater> weapons)
        {
            if (weapons != null && weapons.Count > 0)
            {
                foreach (DamageCreater weapon in weapons)
                {
                    if (weapon)
                        weapon.DealDamage();
                }
            }
        }

        public void RemoveWeapon(DamageCreater wp)
        {
            if (m_weapons != null && m_weapons.Count > 0 && m_weapons.Contains(wp))
            {
                m_weapons.Remove(wp);
            }
        }

        #region FSM
        /// <summary>
        /// Initialzie fsm and set default fsm state
        /// </summary>
        protected void FSMInitialize(MonoBehaviour monoBehaviour)
        {
            m_fsm = StateMachine<States>.Initialize(monoBehaviour);

            m_fsm.ChangeState(States.Idle);
        }

        /// <summary>
        /// Get FSM State's name
        /// </summary>
        public string GetFsmStateName()
        {
            return m_fsm.State.ToString();
        }

        protected IEnumerator PatrolGotoDelay()
        {
            float delay = 0;

            var idleClip = Helper.GetClip(anim, idleState.clipName);

            if (idleClip)
                delay = idleClip.length * 2;

            yield return new WaitForSeconds(delay);

            m_fsm.ChangeState(States.Patrol);
        }

        protected IEnumerator IdleCheck()
        {
            float rate = Random.Range(0f, 1f);

            if (rate <= 0.3f)
                m_fsm.ChangeState(States.Idle);

            yield return new WaitForSeconds(5f);

            m_idleChecked = false;
        }

        protected virtual void Idle_Enter()
        {
            StartCoroutine(PatrolGotoDelay());
        }

        protected virtual void Idle_Update()
        {
            Helper.PlayAnimatorState(anim, idleState.layerIndex, idleState.name);

            if (m_playerDetected)
                m_fsm.ChangeState(States.Chase);
        }

        protected virtual void Idle_Exit()
        {

        }

        protected virtual void Idle_Finally()
        {

        }

        protected virtual void Patrol_Enter()
        {
        }

        protected virtual void Patrol_Update()
        {
            if (!m_idleChecked && !alwaysFollow)
            {
                m_idleChecked = true;

                StartCoroutine(IdleCheck());
            }

            m_curSpeed = patrolSpeed.GetValue();

            if (m_playerDetected)
                m_fsm.ChangeState(States.Chase);

            Helper.PlayAnimatorState(anim, patrolState.layerIndex, patrolState.name);
        }

        protected virtual void Patrol_FixedUpdate()
        {
            Helper.PlayAnimatorState(anim, patrolState.layerIndex, patrolState.name);
        }

        protected virtual void Patrol_Exit()
        {

        }

        protected virtual void PatrolFinally()
        {

        }

        protected virtual void Chase_Enter()
        {

        }

        protected virtual void Chase_Update()
        {
            m_curSpeed = chaseSpeed.GetValue();

            if (featuresManager && featuresManager.ActiveFeature != null)
            {
                if (ActiveFeature == null)
                    ActiveFeature = featuresManager.ActiveFeature;
                m_fsm.ChangeState(States.Action);
            }
            else if (!m_playerDetected)
            {
                m_fsm.ChangeState(States.Patrol);
            }


            Helper.PlayAnimatorState(anim, chaseState.layerIndex, chaseState.name);
        }

        protected virtual void Chase_FixedUpdate()
        {
            Helper.PlayAnimatorState(anim, chaseState.layerIndex, chaseState.name);
        }


        protected virtual void Chase_Exit()
        {

        }

        protected virtual void Chase_Finally()
        {

        }

        protected virtual void TakeDamage_Enter()
        {
            StartCoroutine(TakeDamageDelay());
        }

        protected virtual void Action_Enter()
        {

        }

        protected virtual void Action_FixedUpdate()
        {
            if (ActiveFeature != null)
            {
                m_curSpeed = ActiveFeature.moveSpeed;

                ActiveFeature.Trigger();
            }
        }

        protected virtual void Action_Update()
        {
            if (ActiveFeature != null)
            {
                m_curSpeed = ActiveFeature.moveSpeed;

                ActiveFeature.Trigger();
            }
        }

        protected virtual void Action_Exit()
        {

        }

        protected virtual void Action_Finally()
        {

        }

        protected virtual void Death_Enter()
        {
            StartCoroutine(Die());
        }

        /// <summary>
        /// Back to patrol fsm state
        /// </summary>
        public void BackToPatrolState()
        {
            m_fsm.ChangeState(States.Patrol);
        }

        /// <summary>
        /// Back to chase fsm state
        /// </summary>
        public void BackToChaseState()
        {
            m_fsm.ChangeState(States.Chase);
        }

        #endregion

        /// <summary>
        /// Draw some visual on Gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = new Color32(255, 0, 15, 50);
            Gizmos.DrawSphere(transform.position, searchRange);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(transform.position.x + maxMoveDistance, transform.position.y, transform.position.z),
                new Vector3(-maxMoveDistance + transform.position.x, transform.position.y, transform.position.z));
        }
    }
}
