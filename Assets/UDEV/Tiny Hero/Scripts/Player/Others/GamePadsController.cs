using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero {
    public class GamePadsController : Singleton<GamePadsController>
    {
        public bool isOnMobile;

        public Joystick shootingStick;

        [Range(0.1f, 1f)]
        public float shootingThreshold;

        bool m_isMLeftBtnPressed;
        bool m_isMRightBtnPressed;

        bool m_isJumpBtnPressed;
        bool m_isJumpBtnHolding;

        bool m_isShootingBtnPressed;
        Vector2 m_shootDir;

        public bool IsMLeftBtnPressed { get => m_isMLeftBtnPressed; set => m_isMLeftBtnPressed = value; }
        public bool IsMRightBtnPressed { get => m_isMRightBtnPressed; set => m_isMRightBtnPressed = value; }
        public bool IsJumpBtnPressed { get => m_isJumpBtnPressed; set => m_isJumpBtnPressed = value; }
        public bool IsJumpBtnHolding { get => m_isJumpBtnHolding; set => m_isJumpBtnHolding = value; }
        public bool IsShootingBtnPressed { get => m_isShootingBtnPressed; }
        public Vector2 ShootDir { get => m_shootDir; set => m_shootDir = value; }

        public override void Awake()
        {
            MakeSingleton(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (isOnMobile) return;

            PCMove();
            PCJump();
        }

        public void Disable()
        {
            m_isJumpBtnPressed = false;
            m_isJumpBtnHolding = false;
            m_isMLeftBtnPressed = false;
            m_isMRightBtnPressed = false;
            m_isShootingBtnPressed = false;
        }

        private void LateUpdate()
        {
            if (isOnMobile)
            {
                if (shootingStick)
                {
                    m_isShootingBtnPressed = shootingStick.progress >= shootingThreshold && shootingStick.isHolding;
                    m_shootDir = new Vector2(shootingStick.xValue, shootingStick.yValue);
                }
            }
            else
            {
                m_isShootingBtnPressed = Input.GetButton("Fire1");
            }
        }

        void PCMove()
        {
            float xDir = Input.GetAxisRaw("Horizontal");

            m_isMLeftBtnPressed = xDir < 0 ? true : false;

            m_isMRightBtnPressed = xDir > 0 ? true : false;
        }

        void PCJump()
        {
            m_isJumpBtnPressed = Input.GetKeyDown(KeyCode.UpArrow);

            m_isJumpBtnHolding = Input.GetKey(KeyCode.UpArrow);
        }
    }
}
