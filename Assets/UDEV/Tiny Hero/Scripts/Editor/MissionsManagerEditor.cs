using UnityEditor;
using UnityEngine;

namespace UDEV.TinyHero {
    [CustomEditor(typeof(MissionsManager))]
    public class MissionManagerEditor : Editor
    {
        MissionsManager missionMng;

        string[] ids;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            missionMng = (MissionsManager)target;

            if (missionMng.missions != null && missionMng.missions.Length > 0)
            {
                ids = new string[missionMng.missions.Length];

                for (int i = 0; i < missionMng.missions.Length; i++)
                {
                    if (missionMng.missions[i] != null &&
                        missionMng.missions[i].missionController != null)
                    {
                        ids[i] = missionMng.missions[i].missionController.id;

                        missionMng.missions[i].missionOrder = (i + 1);
                    }
                }
            }

            if (GUI.changed)
            {
                Utils.SaveDataToFile<string[]>(GameConsts.EDITOR_DATA_PATH, "MissionIds.dat", ids);
            }
        }
    }
}