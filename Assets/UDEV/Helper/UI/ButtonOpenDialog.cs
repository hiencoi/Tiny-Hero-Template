using UnityEngine;
using System.Collections;

namespace UDEV
{
    public class ButtonOpenDialog : MyButton
    {

        public DialogType dialogType;
        public DialogShow dialogShow;

        public override void OnButtonClick()
        {
            base.OnButtonClick();
            DialogController.Ins.ShowDialog(dialogType, dialogShow);
        }
    }
}
