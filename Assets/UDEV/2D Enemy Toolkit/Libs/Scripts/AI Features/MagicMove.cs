using UnityEngine;
using UDEV.SPM;

namespace UDEV.AI2D
{
    public class MagicMove : aiFeature
    {
        public enum AIType
        {
            OnAir,
            OnGround
        }

        [Header("Feature Settings:")]
        public float distCondition; // Distance feature can be trigger
        public float minDistCondition; //Min Distance feature can be trigger
        public AIType aiType;
        public float distanceMulti = 1;
        [PoolerKeys(target = PoolerTarget.VFX)]
        public string moveEffect;

        Vector3 m_randomPos;
        bool m_haveFlyingPos;
        bool m_haveMovingPos;

        public AudioClip moveSound;

        protected override bool TriggerCondition()
        {
            return m_aiController.DistToPlayer <= distCondition && m_aiController.DistToPlayer > minDistCondition;
        }

        protected override void OnTriggerEnter()
        {
            switch (aiType)
            {
                case AIType.OnAir:
                    OnAirMove();
                    break;
                case AIType.OnGround:
                    OnGroundMove();
                    break;
            }
        }

        void OnAirMove()
        {
            OnAirAI onAirAI = m_aiController.GetComponent<OnAirAI>();

            if (onAirAI)
            {
                Vector3 pp = m_aiController.Player.transform.position;

                float distToMe = 0;

                int count = 0;

                while (distToMe <= m_aiController.maxMoveDistance + (distanceMulti / 2))
                {
                    float maxX = transform.position.x + m_aiController.maxMoveDistance;
                    float minX = -m_aiController.maxMoveDistance + transform.position.x;

                    float maxY = transform.position.y + m_aiController.maxMoveDistance;
                    float minY = -m_aiController.Player.transform.position.y;

                    float randPosX = Random.Range(minX * distanceMulti, maxX * distanceMulti);

                    float randPosY = Random.Range(minY * distanceMulti, maxX * distanceMulti);

                    m_randomPos = new Vector3(randPosX, randPosY, transform.position.z);

                    distToMe = Vector2.Distance(transform.position, m_randomPos);

                    count++;

                    m_haveFlyingPos = true;

                    if (count >= 100)
                    {
                        m_haveFlyingPos = false;
                        return;
                    };
                }

                if (m_haveFlyingPos)
                {
                    PoolersManager.Ins.Spawn(PoolerTarget.VFX, moveEffect, m_aiController.transform.position, Quaternion.identity);

                    AudioController.Ins.PlaySound(moveSound);

                    onAirAI.transform.position = m_randomPos;
                }
            }
        }

        void OnGroundMove()
        {
            OnGroundAI onGroundAi = m_aiController.GetComponent<OnGroundAI>();

            if (onGroundAi)
            {
                Vector3 pp = m_aiController.Player.transform.position;

                float disToMe = 0;

                int count = 0;

                while (disToMe <= m_aiController.maxMoveDistance)
                {
                    m_haveMovingPos = true;

                    float maxX = pp.x + m_aiController.maxMoveDistance;
                    float minX = -m_aiController.maxMoveDistance + pp.x;

                    m_randomPos = Random.Range(0f, 1f) >= 0.5f ?
                        new Vector3(maxX * distanceMulti, transform.position.y, transform.position.z) :
                        new Vector3(minX * distanceMulti, transform.position.y, transform.position.z);

                    disToMe = Vector2.Distance(m_randomPos, transform.position);

                    count++;

                    if (count >= 100)
                    {
                        m_haveMovingPos = false;
                        return;
                    }
                }

                if (m_haveMovingPos)
                {
                    PoolersManager.Ins.Spawn(PoolerTarget.VFX, moveEffect, m_aiController.transform.position, Quaternion.identity);

                    AudioController.Ins.PlaySound(moveSound);

                    onGroundAi.transform.position = m_randomPos;

                    m_aiController.rb.velocity = Vector3.zero;
                }
            }
        }

        protected override void ResetToDefault()
        {
            base.ResetToDefault();

            m_haveFlyingPos = false;
            m_haveMovingPos = false;
        }
    }
}
