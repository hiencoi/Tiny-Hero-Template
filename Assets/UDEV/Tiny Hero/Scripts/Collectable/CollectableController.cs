using UnityEngine;
using System.Collections;

namespace UDEV.TinyHero
{
    [RequireComponent(typeof(Rigidbody2D), typeof(AutoDestroy))]
    public class CollectableController : MonoBehaviour
    {
        [Header("Base Settings:")]
        [TagList]
        public string playerTag;

        public FlashVfx flashVfx;

        public Collider2D col2d;

        float m_livingTime;

        float m_timeToReduceVelocity = 2f;
        bool m_velocityReduced;
        bool m_canMove;

        protected Player m_player;
        Rigidbody2D m_rb;
        AutoDestroy autoDestroy;

        private void OnDisable()
        {
            m_velocityReduced = false;
            m_timeToReduceVelocity = 2f;
            m_canMove = false;

            if (col2d)
                col2d.isTrigger = false;
            if (m_rb)
                m_rb.isKinematic = false;

            if (autoDestroy)
            {
                m_livingTime = autoDestroy.delay;
            }

            if (flashVfx)
            {
                flashVfx.StopFlash();
                flashVfx.SetSpritesAlpha(flashVfx.normalColor);
            }
        }

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody2D>();
            autoDestroy = GetComponent<AutoDestroy>();

            if (autoDestroy)
            {
                m_livingTime = autoDestroy.delay;
            }
        }

        private void Start()
        {
            m_player = GameManager.Ins.Player;
        }

        private void Update()
        {
            ReduceVelocity();

            m_livingTime -= Time.deltaTime;

            if(m_livingTime <= 2f)
            {
                if (flashVfx)
                    flashVfx.Flash(2f);
            }

            if(Vector2.Distance(m_player.transform.position, transform.position) <= 3
                || m_canMove)
            {
                m_canMove = true;
                if (col2d)
                    col2d.isTrigger = true;
                if (m_rb)
                    m_rb.isKinematic = true;
                if (Vector2.Distance(m_player.transform.position, transform.position) <= 1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, m_player.transform.position, 1000f * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, m_player.transform.position, 10f * Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// Reduce velocity of collectable when spawn
        /// </summary>
        private void ReduceVelocity()
        {
            if (m_timeToReduceVelocity > 0)
            {
                m_timeToReduceVelocity -= Time.deltaTime;
            }
            else
            {
                if (!m_velocityReduced)
                {
                    m_rb.velocity = Vector3.zero;
                    m_rb.angularVelocity = 0f;
                    m_velocityReduced = true;
                }
            }
        }

        /// <summary>
        /// Excute when player hit collectable
        /// </summary>
        protected virtual void CollectableTrigger()
        {

        }

        private void OnCollisionEnter2D(Collision2D target)
        {
            if (target.gameObject.CompareTag(playerTag))
            {
                CollectableTrigger();
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D target)
        {
            if (target.gameObject.CompareTag(playerTag))
            {
                CollectableTrigger();
                gameObject.SetActive(false);
            }
        }
    }
}
