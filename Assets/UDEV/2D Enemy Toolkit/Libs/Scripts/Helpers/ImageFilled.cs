using UnityEngine;
using UnityEngine.UI;

namespace UDEV.AI2D
{

    public class ImageFilled : MonoBehaviour
    {

        public Image filledImg;

        Transform m_root;

        public Transform Root { get => m_root; set => m_root = value; }

        public void UpdateValue(float curVal, float totalVal, bool isReverse = false)
        {
            if (filledImg)
            {
                float rate = 0;

                if (isReverse)
                {
                    rate = 1f - (curVal / totalVal);
                }
                else
                {
                    rate = curVal / totalVal;
                }

                filledImg.fillAmount = rate;
            }
        }

        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        private void Update()
        {
            if (m_root)
            {
                transform.localRotation = m_root.rotation;
            }
        }
    }

}
