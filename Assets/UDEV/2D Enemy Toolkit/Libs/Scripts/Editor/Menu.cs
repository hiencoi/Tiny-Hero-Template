using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace UDEV.AI2D
{

    public class Menu : Editor
    {
        [MenuItem("Assets/Add Feature Into Prefab", false, 690)]
        static public void AddFeatureIntoPrefab()
        {
            string path = "";
            string name = "";
            MonoScript renderedScript = null;

            if (Selection.activeObject is MonoScript)
            {
                renderedScript = (MonoScript)Selection.activeObject;
                name = renderedScript.GetClass().ToString();
                path = AssetDatabase.GetAssetPath(renderedScript);

                string[] pathArr = path.Split("/"[0]);

                path = "";

                for (int i = 0; i < pathArr.Length - 1; i++)
                {
                    path += pathArr[i] + "/";
                }

                var type = Utils.GetType(name);

                if (type != null)
                {
                    string fileName = pathArr[pathArr.Length - 1].Replace(".cs", "");

                    GameObject feature = new GameObject(fileName);

                    feature.AddComponent(type);

                    var check = feature.GetComponent<aiFeature>();

                    if (check != null)
                    {
                        string basePath = path + "Prefabs/";
                        string finalPath = basePath + fileName + ".prefab";
                        Utils.CreateMissingDirectory(basePath);
                        if (AssetDatabase.LoadAssetAtPath(finalPath, typeof(GameObject)))
                        {
                            if (EditorUtility.DisplayDialog("Are you sure?",
                                "The Prefab already exists. Do you want to overwrite it?",
                                "Yes",
                                "No"))
                            {
                                var f = PrefabUtility.SaveAsPrefabAsset(feature, finalPath);
                                if (f)
                                    AssetDatabase.SetLabels(f, new string[] { "aiFeature" });
                                Debug.Log("Feature added into prefab!.Please check in Prefabs folder.");
                            }
                        }
                        else
                        {
                            var f = PrefabUtility.SaveAsPrefabAsset(feature, finalPath);
                            if (f)
                                AssetDatabase.SetLabels(f, new string[] { "aiFeature" });
                            Debug.Log("Feature added into prefab!.Please check in Prefabs folder.");
                        }
                    }
                    else
                        Debug.LogWarning("Please Select AI Feature Script!.");

                    DestroyImmediate(feature);
                }
                else
                {
                    Debug.LogWarning("Please Select AI Feature Script!.");
                }
            }
            else
            {
                Debug.LogWarning("Please Select AI Feature Script!.");
            }
        }
    }
}
