using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UDEV.AI2D
{
    public class OnGroundAI : aiBase
    {
        [Header("Move Settings:")]
        public Facing onStartMoveTo; //Direction to move when start
        Vector3 m_startPos; //Starting position of ai
        float m_limitXR; //Max move right postion ai can move
        float m_limitXL; //Max move left position ai can move
        [Header("Check On Ground:")]
        [LayerList]
        public int groundLayer; //Ground layer
        public float groundCheckScaleX = 0.1f; //Scale x to dection slope
        public float groundCheckScaleY = 0.1f; //Scale y to dection slope
        public float groundCheckDistance = 1f; //Distance to check ai on ground or not
        public Vector3 groundCheckOffset; //Offset for ground checking method

        bool m_isGrounded; //Variable check ai on ground or not
        bool m_canMoveLeft; //AI moving left
        bool m_canMoveRight; //AI moving right
        bool m_dirSwitched; //Moving direction of ai changed
        bool m_canFollow = true; //Variable check ai can follow player or not
        Vector2 m_followDir;

        #region Getter_Setter
        public bool IsGrounded { get => m_isGrounded; set => m_isGrounded = value; }
        #endregion

        public override void OnEnable()
        {
            base.OnEnable();

            if (IsDead)
            {
                IsDead = false;
                Init();
                FSMInitialize(this);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            //Initialize FSM
            FSMInitialize(this);

            //Caculate limit of move position
            m_startPos = transform.position;
            m_limitXR = m_startPos.x + maxMoveDistance;
            m_limitXL = -maxMoveDistance + m_startPos.x;

            //Set direction for ai going to move on start
            if (onStartMoveTo == Facing.LEFT)
            {
                m_canMoveLeft = true;
                m_canMoveRight = false;
            }
            else if (onStartMoveTo == Facing.RIGHT)
            {
                m_canMoveRight = true;
                m_canMoveLeft = false;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            CheckOnGround();
        }

        /// <summary>
        /// Method Check ai on ground or not
        /// </summary>
        void CheckOnGround()
        {
            m_isGrounded = false;
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + groundCheckOffset, new Vector2(groundCheckScaleX, groundCheckScaleY), 0, Vector2.down, groundCheckDistance, 1 << groundLayer);
            if (hit.collider != null)
            {
                m_isGrounded = true;

                m_dirSwitched = false;

                if (hit.normal.y != 0)
                {
                    var slope = -hit.normal.x / hit.normal.y;

                    var rotZ = slope * Mathf.Rad2Deg;

                    transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y,
                        rotZ);

                    if (rb.velocity.y > 0)
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * -1f);
                }
            }
            else
            {
                m_isGrounded = false;
            }
        }

        /// <summary>
        /// Method make ai move follow player
        /// </summary>
        void MoveFollowPlayer()
        {
            if (IsFreeze || m_canKnockback) return;

            if (!m_isGrounded)
            {
                m_followDir = PlayerDir;

                Flip(m_followDir);

                rb.velocity = m_followDir.x > 0 ?
                        new Vector2(CurSpeed, rb.velocity.y) :
                        new Vector2(-CurSpeed, rb.velocity.y);
            }

            //if distance to player greate than min distance condition of ai feature
            //and m_canFollow variable is true and follow angle limit less then 45 deg
            //ai can follow player
            if (m_canFollow && PlayerAngle < 45)
            {
                if (ActiveFeature && !ActiveFeature.IsActive)
                    m_followDir = PlayerDir;

                if (ActiveFeature == null)
                    m_followDir = PlayerDir;

                if (rb)
                {
                    if (m_fsm.State != States.Action)
                    {
                        Flip(PlayerDir);
                    }

                    rb.velocity = m_followDir.x > 0 ?
                        new Vector2(CurSpeed, rb.velocity.y) :
                        new Vector2(-CurSpeed, rb.velocity.y);
                }
            }
            else
            {
                //If ai can't follow player and dont attacking player
                //Trigger FollowDelay method
                FollowDelay();
            }
        }

        /// <summary>
        /// Follow Delay Method
        /// After method finish ai can follow player if not ai can not follow player
        /// </summary>
        void FollowDelay()
        {
            if (rb)
            {
                if (ActiveFeature && ActiveFeature.moveSpeed > 0)
                {
                    Flip(m_followDir);
                }
                else
                {
                    if(m_fsm.State == States.Action)
                    {
                        if(m_canFollow)
                            Flip(PlayerDir);
                    }
                    else
                    {
                        Flip(m_followDir);
                    }
                }

                rb.velocity = m_followDir.x > 0 ?
                    new Vector2(CurSpeed, rb.velocity.y) :
                    new Vector2(-CurSpeed, rb.velocity.y);
            }

            if (m_canFollow)
            {
                StartCoroutine(CanFollowCo(0.5f));
            }
        }

        /// <summary>
        /// Can Follow method coroutine
        /// </summary>
        protected IEnumerator CanFollowCo(float delay)
        {
            m_canFollow = false;
            yield return new WaitForSeconds(delay);
            m_canFollow = true;
        }

        /// <summary>
        /// Method make ai move in patrol state
        /// </summary>
        void PatrolMove()
        {
            if (IsFreeze || m_canKnockback) return;

            SwitchPatrolMoveDir();

            //Move ai to left
            if (m_canMoveLeft)
            {
                PatrolMoveLeft();

                if (groundCheckOffset.x < 0) return;

                //Flip ground checking offset to left
                groundCheckOffset = groundCheckOffset.x > 0 ?
                    new Vector3(-groundCheckOffset.x, groundCheckOffset.y, groundCheckOffset.z) :
                    groundCheckOffset;
            }// Move ai to right
            else if (m_canMoveRight)
            {
                PatrolMoveRight();

                if (groundCheckOffset.x > 0) return;

                //Flip ground checking offset to right
                groundCheckOffset = groundCheckOffset.x > 0 ?
                    groundCheckOffset :
                    new Vector3(-groundCheckOffset.x, groundCheckOffset.y, groundCheckOffset.z);
            }
        }

        /// <summary>
        /// Switch Patrol move direction method
        /// </summary>
        void SwitchPatrolMoveDir()
        {
            //If position of ai great than limit of move right position make ai move left
            if (transform.position.x >= m_limitXR && m_isGrounded)
            {
                m_canMoveLeft = true;
                m_canMoveRight = false;
            }

            //If position of ai less than limit of move left position make ai move right
            if (transform.position.x <= m_limitXL && m_isGrounded)
            {
                m_canMoveRight = true;
                m_canMoveLeft = false;
            }

            //If ai not on the ground and moving left make ai move right
            if (!m_isGrounded && m_canMoveLeft && !m_dirSwitched)
            {
                m_canMoveLeft = false;
                m_canMoveRight = true;
                m_dirSwitched = true;
            }

            //If ai not on the ground and moving right make ai move left
            if (!m_isGrounded && m_canMoveRight && !m_dirSwitched)
            {
                m_canMoveLeft = true;
                m_canMoveRight = false;
                m_dirSwitched = true;
            }
        }

        /// <summary>
        /// Method move ai to left
        /// </summary>
        void PatrolMoveLeft()
        {
            Flip(Vector3.left);

            rb.velocity = new Vector2(-CurSpeed, rb.velocity.y);
        }

        /// <summary>
        /// Method move ai to right
        /// </summary>
        void PatrolMoveRight()
        {
            Flip(Vector3.right);

            rb.velocity = new Vector2(CurSpeed, rb.velocity.y);
        }

        #region FSM
        protected override void Idle_Enter()
        {
            base.Idle_Enter();
        }

        protected override void Idle_Update()
        {
            base.Idle_Update();
        }

        protected override void Idle_Exit()
        {
            base.Idle_Exit();
        }

        protected override void Idle_Finally()
        {
            base.Idle_Finally();
        }

        protected override void PatrolFinally()
        {
            base.PatrolFinally();
        }

        protected override void Patrol_Enter()
        {
            base.Patrol_Enter();
        }

        protected override void Patrol_Exit()
        {
            base.Patrol_Exit();
        }

        protected override void Patrol_FixedUpdate()
        {
            base.Patrol_FixedUpdate();

            if (alwaysFollow)
            {
                if (m_canFollow)
                    MoveFollowPlayer();
                else
                    PatrolMove();
            }
            else
            {
                PatrolMove();
            }
        }

        protected override void Patrol_Update()
        {
            if (!m_idleChecked && !alwaysFollow)
            {
                m_idleChecked = true;

                StartCoroutine(IdleCheck());
            }

            CurSpeed = patrolSpeed.GetValue();

            if (m_canFollow && m_playerDetected)
            {
                BackToChaseState();
            }
            Helper.PlayAnimatorState(anim, patrolState.layerIndex, patrolState.name);
        }

        protected override void Chase_Enter()
        {
            base.Chase_Enter();
            PlayerDir = Player.transform.position - transform.position;
            PlayerDir.Normalize();
            m_followDir = PlayerDir;
        }

        protected override void Chase_Exit()
        {
            base.Chase_Exit();
        }

        protected override void Chase_Finally()
        {
            base.Chase_Finally();
        }

        protected override void Chase_Update()
        {
            base.Chase_Update();
        }

        protected override void Chase_FixedUpdate()
        {
            base.Chase_FixedUpdate();

            MoveFollowPlayer();
        }

        protected override void TakeDamage_Enter()
        {
            base.TakeDamage_Enter();
        }

        protected override void Action_Enter()
        {
            base.Action_Enter();

            Flip(PlayerDir);
        }

        protected override void Action_Exit()
        {
            base.Action_Exit();
        }

        protected override void Action_Finally()
        {
            base.Action_Finally();
        }

        protected override void Action_FixedUpdate()
        {
            base.Action_FixedUpdate();

            MoveFollowPlayer();
        }

        protected override void Death_Enter()
        {
            base.Death_Enter();
        }
        #endregion

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + groundCheckOffset, new Vector3(transform.position.x + groundCheckOffset.x,
                transform.position.y + groundCheckOffset.y + -groundCheckDistance,
                transform.position.z));
        }
    }
}
