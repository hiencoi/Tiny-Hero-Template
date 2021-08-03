using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace UDEV.TinyHero
{
    [CustomPropertyDrawer(typeof(MissionIdsAttribute), true)]
    public class MissionIdsDrawer : PropertyDrawer
    {
        int _choiceIndex = 0;
        string[] missionIndexStr;
        string[] missionIds;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetMissionIds();
            if (missionIds != null && missionIds.Length > 0)
            {
                _choiceIndex = Array.IndexOf(missionIds, property.stringValue);

                if (_choiceIndex < 0)
                {
                    _choiceIndex = 0;
                    property.stringValue = missionIds[_choiceIndex];
                }

                _choiceIndex = EditorGUI.Popup(position, label.text, _choiceIndex, missionIndexStr);

                property.stringValue = missionIds[_choiceIndex];
            }
            else
            {
                EditorGUI.BeginProperty(position, label, property);
                property.intValue = EditorGUI.IntField(position, label.text, property.intValue);
                EditorGUI.EndProperty();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        void GetMissionIds()
        {
            missionIds = new string[] { };

            string filePath = GameConsts.EDITOR_DATA_PATH + "MissionIds.dat";

            missionIds = Utils.LoadDataFromFile<string[]>(filePath, missionIds);

            if (missionIds != null && missionIds.Length > 0)
            {
                missionIndexStr = new string[missionIds.Length];

                for (int i = 0; i < missionIds.Length; i++)
                {
                    missionIndexStr[i] = (i + 1).ToString();
                }
            }
        }
    }
}
