using UnityEngine;

namespace UDEV.TinyHero
{
    public class ParallaxFollowCam : MonoBehaviour
    {
        float m_length, m_startPos;
        Transform m_cam;
        public float parallaxEffect;

        private void Start()
        {
            m_cam = Camera.main.transform;
            m_startPos = transform.position.x;
            m_length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void FixedUpdate()
        {
            float temp = (m_cam.position.x * (1 - parallaxEffect));
            float dist = (m_cam.position.x * parallaxEffect);
            transform.position = new Vector3(m_startPos + dist, transform.position.y, transform.position.z);

            if (temp > (m_startPos + m_length)) m_startPos += m_length;
            else if (temp < (m_startPos - m_length)) m_startPos -= m_length;
        }
    }
}
