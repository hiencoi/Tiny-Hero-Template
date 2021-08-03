﻿using UnityEngine;
using System.Collections;

namespace UDEV
{
    public class ButtonGotoScene : MyButton
    {
        public int sceneIndex;
        public bool useScreenFader;
        public bool useKeyCode;
        public KeyCode keyCode;

        public override void OnButtonClick()
        {
            base.OnButtonClick();
            CUtils.LoadScene(sceneIndex, useScreenFader);
        }

        private void Update()
        {
            if (useKeyCode && Input.GetKeyDown(keyCode) && !DialogController.Ins.IsDialogShowing())
            {
                OnButtonClick();
            }
        }
    }
}
