using UnityEditor;
using System;
using UnityEngine;

namespace UDEV.AI2D {
    [CustomPropertyDrawer(typeof(aiFeaturesAttribute))]
    public class aiFeaturesDrawer : PropertyDrawer
    {

        string[] namesArr;
        string[] featureNames;
        int _selectedIndex = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Behaviour beh = property.serializedObject.targetObject as Behaviour;

            aiFeaturesManager featureManager = beh.transform.root.GetComponent<aiFeaturesManager>();

            if (featureManager != null)
            {
                var names = featureManager.GetFeatureNames();

                if (names != null)
                {
                    namesArr = names.ToArray();

                    if (namesArr != null && namesArr.Length > 0)
                    {
                        featureNames = new string[namesArr.Length + 1];

                        for (int i = 0; i < namesArr.Length + 1; i++)
                        {
                            featureNames[i] = i == 0 ? "None" : namesArr[i - 1];
                        }

                        if (featureNames != null && featureNames.Length > 0)
                        {
                            var featureSelected1 = featureManager.GetFeatureById(property.stringValue);

                            if (featureSelected1 != null)
                            {
                                _selectedIndex = Array.IndexOf(featureNames, featureSelected1.name);

                                _selectedIndex = _selectedIndex < 0 ? 0 : _selectedIndex;

                            }

                            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, featureNames);


                            var featureSelected2 = featureManager.GetFeatureByName(featureNames[_selectedIndex]);

                            if (featureSelected2 != null)
                            {
                                property.stringValue = featureSelected2.id;
                            }
                            else
                            {
                                property.stringValue = "";
                            }
                        }
                    }
                }
            }

            if (featureManager == null || namesArr == null || namesArr.Length <= 0)
            {
                EditorGUI.BeginProperty(position, label, property);
                property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
                EditorGUI.EndProperty();
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
