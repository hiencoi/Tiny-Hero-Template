using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GridSlider
{
    public class GridSliderPageUI : MonoBehaviour
    {
        [HideInInspector]
        public float with, height;
        public GridLayoutGroup pageContent;

        private void Update()
        {
            RectTransform rect = pageContent.GetComponent<RectTransform>();

            with = rect.rect.width;
            height = rect.rect.height;
        }
    }
}
