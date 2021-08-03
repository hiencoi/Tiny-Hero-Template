using UnityEngine;
using System.Collections;
using System;

namespace UDEV
{
    public class YesNoDialog : Dialog
    {
        public Action onYesClick;
        public Action onNoClick;
        public virtual void OnYesClick()
        {
            if (onYesClick != null) onYesClick();
            Close();
        }

        public virtual void OnNoClick()
        {
            if (onNoClick != null) onNoClick();
            Close();
        }
    }
}
