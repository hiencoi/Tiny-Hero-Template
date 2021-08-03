using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace UDEV.TinyHero
{
    [CustomPropertyDrawer(typeof(SkillIdsAttribute), true)]
    public class SkillIdsDrawer : PropertyDrawer
    {
        int _choiceIndex = 0;
        string[] skillNames;
        string[] skillIds;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetData();
            if (skillIds != null && skillIds.Length > 0)
            {
                _choiceIndex = Array.IndexOf(skillIds, property.stringValue);

                if (_choiceIndex < 0)
                {
                    _choiceIndex = 0;
                    property.stringValue = skillIds[_choiceIndex];
                }

                _choiceIndex = EditorGUI.Popup(position, label.text, _choiceIndex, skillNames);

                property.stringValue = skillIds[_choiceIndex];
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

        void GetData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string filePath = GameConsts.EDITOR_DATA_PATH + "SkillIds.dat";

            data = Utils.LoadDataFromFile<Dictionary<string, string>>(filePath, data);

            if(data != null && data.Count > 0)
            {
                skillIds = data.Values.ToArray();

                skillNames = data.Keys.ToArray();
            }
        }
    }
}
