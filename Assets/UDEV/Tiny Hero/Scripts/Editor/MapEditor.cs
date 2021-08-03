using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UDEV.TinyHero
{
    [CustomEditor(typeof(Map))]
    public class MapEditor : Editor
    {
        Map m_map;
        SerializedProperty playerSpawnPointProp;
        SerializedProperty enemySpawnPointsProp;

        public override void OnInspectorGUI()
        {
            m_map = (Map)target;

            playerSpawnPointProp = serializedObject.FindProperty("playerSpawnPoint");
            enemySpawnPointsProp = serializedObject.FindProperty("enemySpawnPoints");

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(playerSpawnPointProp, new GUIContent("Player Spawn Point: "));
            EditorGUILayout.EndHorizontal();

            List<GameObject> enemySpawnPoints = Utils.FindObjectInChilds(m_map.gameObject, "Spawner");

            if (enemySpawnPoints != null && enemySpawnPoints.Count > 0)
            {
                System.Array.Resize(ref m_map.enemySpawnPoints, enemySpawnPoints.Count);
            }

            var enemyPointsPropSize = enemySpawnPointsProp.arraySize;

            if (enemySpawnPointsProp != null && enemyPointsPropSize > 0 &&
               enemyPointsPropSize == enemySpawnPoints.Count)
            {
                EditorGUILayout.LabelField("Enemy Spawn Points:");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();

                if(enemySpawnPoints != null && enemySpawnPoints.Count > 0)
                {
                    for (int i = 0; i < enemySpawnPointsProp.arraySize; i++)
                    {
                        var enemySpawnPointProp = enemySpawnPointsProp.GetArrayElementAtIndex(i);

                        if (enemySpawnPointProp != null && enemySpawnPoints[i] != null)
                        {
                            m_map.enemySpawnPoints[i] = enemySpawnPoints[i].transform;
                            EditorGUILayout.PropertyField(enemySpawnPointProp, new GUIContent(enemySpawnPoints[i].name));
                        }
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
