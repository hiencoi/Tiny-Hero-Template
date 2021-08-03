using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.TinyHero {
    public class MissonGotoButton : ButtonGotoScene
    {
        public Image state;
        public Color locked;
        public Color unlocked;

        [MissionIds]
        public string mission;

        protected override void Start()
        {
            base.Start();

            UpdateStateUI();
        }

        void UpdateStateUI()
        {
            if (Prefs.IsMissionUnlocked(mission))
            {
                if (state)
                    state.color = unlocked;
            }
            else
            {
                if (state)
                    state.color = locked;
            }
        }

        public override void OnButtonClick()
        {
            if (Prefs.IsMissionUnlocked(mission))
            {
                MissionsManager.Ins.CurMissionId = mission;
                CUtils.LoadScene(sceneIndex, useScreenFader);
            }
        }
    }
}
