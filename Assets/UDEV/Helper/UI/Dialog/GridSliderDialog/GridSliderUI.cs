using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GridSlider
{
    public class GridSliderUI : MonoBehaviour
    {
        public GridLayoutGroup configs;

        public GridSliderPageUI page;

        public RectTransform main;

        public RectTransform mask;

        public Button nextPageBtn;

        public Button prevPageBtn;

        public Button closeBtn;
    }
}
