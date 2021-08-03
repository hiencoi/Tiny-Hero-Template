using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class RateGameController : RepeatingAction
    {
        public override void UpdateAction()
        {
            if (Prefs.UserRated) return;

            base.UpdateAction();
        }
    }
}
