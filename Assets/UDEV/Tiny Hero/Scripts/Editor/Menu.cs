using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace UDEV.TinyHero {
    public class Menu : MonoBehaviour
    {
        [MenuItem(GameConsts.PROJECT_NAME + "/Clear all playerprefs")]
        static void ClearAllPlayerprefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [MenuItem(GameConsts.PROJECT_NAME + "/Add 1000000 coins")]
        static void AddCoins()
        {
            CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
            Prefs.coins += 1000000;
        }

        [MenuItem(GameConsts.PROJECT_NAME + "/Remove all coins")]
        static void RemoveCoins()
        {
            CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
            Prefs.coins = 0;
        }

        [MenuItem("Assets/Create/UDEV/" + GameConsts.PROJECT_NAME + "/Create Mission", false, 1)]
        private static void CreateNewMission()
        {
            Object newMission = null;

            string dirPath = GameConsts.MISSION_SAVE_PATH;

            string localPath = GameConsts.MISSION_SAVE_PATH + "NewMission_" + Path.GetRandomFileName() + ".prefab";

            Utils.CreateMissingDirectory(dirPath);

            var go = new GameObject();
            go.AddComponent(typeof(MissionController));

            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            {
                if (EditorUtility.DisplayDialog("Are you sure?",
                    "The Prefab already exists. Do you want to overwrite it?",
                    "Yes",
                    "No"))
                {
                    newMission = CreateNewPrefab(go, localPath);
                }
            }
            else
            {
                newMission = CreateNewPrefab(go, localPath);
            }

            DestroyImmediate(go);

            if(newMission)
                AssetDatabase.SetLabels(newMission, new string[] { "Mission" });
        }

        [MenuItem("Assets/Add Skill Into Prefab", false, 690)]
        static public void AddFeatureIntoPrefab()
        {
            Object newSkill = null;
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

                    var check = feature.GetComponent<Skill>();

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
                                newSkill = CreateNewPrefab(feature, finalPath);
                                if (newSkill)
                                    AssetDatabase.SetLabels(newSkill, new string[] { "Skill" });
                                Debug.Log("Skill added into prefab!.Please check in Prefabs folder.");
                            }
                        }
                        else
                        {
                            newSkill = CreateNewPrefab(feature, finalPath);
                            if (newSkill)
                                AssetDatabase.SetLabels(newSkill, new string[] { "Skill" });
                            Debug.Log("Skill added into prefab!.Please check in Prefabs folder.");
                        }
                        
                    }
                    else
                        Debug.LogWarning("Please Select Skill Script!.");

                    DestroyImmediate(feature);
                }
                else
                {
                    Debug.LogWarning("Please Select Skill Script!.");
                }
            }
            else
            {
                Debug.LogWarning("Please Select Skill Script!.");
            }
        }

        static Object CreateNewPrefab(GameObject obj, string localPath)
        {
            Object prefab = PrefabUtility.SaveAsPrefabAsset(obj, localPath);

            return prefab;
        }

        [MenuItem("Export/MyExport")]
        static void export()
        {
            AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), PlayerSettings.productName + ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
        }
    }
}
