using UnityEngine;
using System.Collections;
using System;

namespace UDEV
{
    public class OkDialog : Dialog
    {
        public Action onOkClick;
        public virtual void OnOkClick()
        {
            if (onOkClick != null) onOkClick();
            Close();
        }
    }
}
