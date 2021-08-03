using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UDEV.TinyHero
{
    [CustomEditor(typeof(SkillsManager))]
    public class SkillsManagerEditor : Editor
    {
        SkillsManager skillMng;

        Dictionary<string, string> ids;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            skillMng = (SkillsManager)target;

            if (skillMng.slots != null && skillMng.slots.Count > 0)
            {
                ids = new Dictionary<string, string>();

                for (int i = 0; i < skillMng.slots.Count; i++)
                {
                    if (skillMng.slots[i] != null &&
                        skillMng.slots[i].skill != null)
                    {
                        string name = skillMng.slots[i].skill.name;
                        ids[name] = skillMng.slots[i].skill.id;
                    }
                }
            }

            if (GUI.changed)
            {
                Utils.SaveDataToFile<Dictionary<string, string>>(GameConsts.EDITOR_DATA_PATH, "SkillIds.dat", ids);
            }
        }
    }

}