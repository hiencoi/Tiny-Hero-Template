using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero {
    public class SkillValueInfoUI : MonoBehaviour
    {
        public Text valueText;

        public void SetValueText(string content)
        {
            if (valueText)
                valueText.text = content;
        }
    }
}
