using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV {
    public class SpritesAutoOrder : MonoBehaviour
    {
        public SpriteRenderer[] renderers;

        private void OnEnable()
        {
            if (renderers != null && renderers.Length > 0)
            {
                int biggestValue = PlayerPrefs.GetInt("sprite_biggest_order", 0);

                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null)
                    {
                        renderers[i].sortingOrder = (i + biggestValue + 1);
                        PlayerPrefs.SetInt("sprite_biggest_order", i + biggestValue + 1);
                    }
                }
            }
        }
    }

}