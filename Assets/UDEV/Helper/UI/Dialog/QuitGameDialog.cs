using UnityEngine;
using System.Collections;

namespace UDEV
{
    public class QuitGameDialog : YesNoDialog
    {
        protected override void Start()
        {
            base.Start();
            onYesClick = Quit;
            onNoClick = PlayOn;
        }

        private void Quit()
        {
            Application.Quit();
        }

        void PlayOn()
        {
            Close();
        }
    }
}
