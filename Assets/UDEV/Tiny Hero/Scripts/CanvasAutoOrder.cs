using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV {
    public class CanvasAutoOrder : MonoBehaviour
    {
        public Canvas canvas;

        private void OnEnable()
        {
            if (canvas)
            {
                int biggestValue = PlayerPrefs.GetInt("sprite_biggest_order", 0);

                canvas.sortingOrder = (biggestValue + 1);

                PlayerPrefs.SetInt("sprite_biggest_order", biggestValue + 1);
            }
        }
    }
}
