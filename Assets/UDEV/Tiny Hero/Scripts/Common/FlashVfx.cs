using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV
{
    public class FlashVfx : MonoBehaviour
    {
        public SpriteRenderer[] spriteRenderers;
        [Range(0f, 0.15f)]
        public float flashRate;
        public Color normalColor;
        public Color flashColor;

        bool m_isFlashing;

        float m_flashingTime;

        Coroutine m_flash;

        public void Flash(float time)
        {
            m_flash = StartCoroutine(FlashCo(time));
        }

        public void StopFlash()
        {
            m_isFlashing = false;

            if (m_flash != null)
                StopCoroutine(m_flash);
        }

        IEnumerator FlashCo(float time)
        {
            if (!m_isFlashing)
            {
                m_flashingTime = time;

                m_isFlashing = true;

                while (m_flashingTime > 0)
                {
                    SetSpritesAlpha(flashColor);
                    yield return new WaitForSeconds(flashRate);
                    SetSpritesAlpha(normalColor);
                    yield return new WaitForSeconds(flashRate);
                    SetSpritesAlpha(flashColor);
                    yield return new WaitForSeconds(flashRate);
                    SetSpritesAlpha(normalColor);
                }

                m_isFlashing = false;
            }
            yield return null;
        }

        public void SetSpritesAlpha(Color color)
        {
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    if (spriteRenderers[i] != null)
                        spriteRenderers[i].color = color;
                }
            }
        }

        private void Update()
        {
            if (m_flashingTime > 0)
                m_flashingTime -= Time.deltaTime;
        }
    }
}
