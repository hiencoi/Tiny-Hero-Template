using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.AI2D
{
    public class OnAirAI : aiBase
    {
        Vector3 m_startPos; // Starting position of ai
        float m_limitXR; // Max left position ai can move
        float m_limitXL; // Min left position ai can move
        float m_limitYT; // Max top position ai can move
        float m_limitYD; // Min bottom postion ai can move
        Vector3 m_flyingPos; //Move position ai move to

        bool m_haveFlyingPos; // Check ai have flying position to move to or not

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

            FSMInitialize(this);

            m_startPos = transform.position;

            FindLimitPos(m_startPos);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            //Flip ai sprite
            if (facing != Facing.NONE)
                Flip(PlayerDir);
        }

        /// <summary>
        /// Get limit move position of ai
        /// </summary>
        void FindLimitPos(Vector3 startPos)
        {
            m_limitXR = startPos.x + maxMoveDistance;
            m_limitXL = -maxMoveDistance + startPos.x;
            m_limitYT = startPos.y + maxMoveDistance;
            m_limitYD = startPos.y;
        }

        /// <summary>
        /// Flying Method
        /// </summary>
        /// <param name="startPos">Starting Position</param>
        /// <param name="lOffset">Offset for left</param>
        /// <param name="rOffset">Offset for right</param>
        /// <param name="tOffset">Offset for top</param>
        /// <param name="dOffset">Offset for bottom</param>
        void Fly(Vector3 startPos, float lOffset = 0, float rOffset = 0, float tOffset = 0, float dOffset = 0)
        {
            if (IsFreeze) return;

            //Caculate move position
            if (!m_haveFlyingPos)
            {
                FindLimitPos(startPos);

                float randomPosX = Random.Range(m_limitXL + lOffset, m_limitXR + rOffset);
                float randomPosY = Random.Range(m_limitYD + dOffset, m_limitYT + tOffset);

                m_haveFlyingPos = true;

                m_flyingPos = new Vector3(randomPosX, randomPosY, transform.position.z);
            }

            //Move ai follow player if distance from ai to move position less than or equal 0.1f
            //if not reset attack rate and moving position
            if (Vector2.Distance(transform.position, m_flyingPos) <= 0.1f)
            {
                m_haveFlyingPos = false;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, m_flyingPos, CurSpeed * Time.deltaTime);
            }
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

        protected override void Patrol_Enter()
        {
            m_haveFlyingPos = false;
        }

        protected override void Patrol_Exit()
        {
            base.Patrol_Exit();
        }

        protected override void PatrolFinally()
        {
            base.PatrolFinally();
        }

        protected override void Patrol_Update()
        {
            if (alwaysFollow)
            {
                float downOffset = -Player.transform.position.y;
                Fly(Player.transform.position, 0, 0, 0, downOffset);
            }
            else
            {
                Fly(m_startPos);
            }

            base.Patrol_Update();
        }

        protected override void Chase_Enter()
        {
            base.Chase_Enter();
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
            float downOffset = -Player.transform.position.y;

            Fly(Player.transform.position, 0, 0, 0, downOffset);

            base.Chase_Update();
        }

        protected override void TakeDamage_Enter()
        {
            base.TakeDamage_Enter();
        }

        protected override void Action_Enter()
        {
            base.Action_Enter();
        }

        protected override void Action_Exit()
        {
            base.Action_Exit();
        }

        protected override void Action_Finally()
        {
            base.Action_Finally();
        }

        protected override void Action_Update()
        {
            base.Action_Update();
        }

        protected override void Death_Enter()
        {
            base.Death_Enter();
        }

        #endregion

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
        }
    }
}
