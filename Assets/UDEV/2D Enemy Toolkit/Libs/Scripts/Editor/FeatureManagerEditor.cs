using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UDEV.AI2D
{
    [CustomEditor(typeof(aiFeaturesManager), true)]
    public class FeatureManagerEditor : Editor
    {
        aiFeaturesManager ftMng;
        internal static List<aiFeatureSlot> fslots;
        int slotCounting;
        Transform root;
        string rootName = "Feature_Manager";
        internal static int slotIdx = 0;
        SerializedProperty slotsProp;

        public override void OnInspectorGUI()
        {
            ftMng = (aiFeaturesManager)target;

            if (IsPrefab(ftMng))
            {
                EditorGUILayout.HelpBox("Open Prefab to edit.", UnityEditor.MessageType.Info, true);
                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ftMng);
                return;
            }
            else
            {
                var abMngClones = ftMng.GetComponents<aiFeaturesManager>();
                if (abMngClones.Length > 1)
                {
                    EditorGUILayout.HelpBox("This Script Not Allow Dupplicate.Please Remove Dupplicated Script!.", UnityEditor.MessageType.Info, true);
                    return;
                }
            }
            CreateOrDelFeature();

            CustomUIDisplay();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(ftMng);
            }
        }

        void CustomUIDisplay()
        {
            if (fslots != null && fslots.Count > 0)
            {
                slotsProp = serializedObject.FindProperty("featureSlots");
                GUIStyle style = new GUIStyle();
                style.normal.background = EditorGUIUtility.whiteTexture;
                style.padding = new RectOffset(5, 5, 5, 5);
                style.border = new RectOffset(5, 5, 5, 5);

                int count = 0;

                for (int i = 0; i < fslots.Count; i++)
                {
                    if(fslots[i] != null)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal(style);

                        EditorGUILayout.BeginVertical();
                        fslots[i].name = EditorGUILayout.TextField("Name", fslots[i].name);
                        EditorGUILayout.BeginHorizontal();
                        fslots[i].feature = (aiFeature)EditorGUILayout.ObjectField("Feature", fslots[i].feature, typeof(aiFeature), true);
                        if (GUILayout.Button("Select Prefab", GUILayout.ExpandHeight(true)))
                        {
                            slotIdx = i;

                            Utils.ShowPicker<GameObject>("l:aiFeature");
                        }
                        EditorGUILayout.EndHorizontal();

                        fslots[i].isLocked = EditorGUILayout.Toggle("Locked", fslots[i].isLocked);
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(25));


                        if (GUILayout.Button(" X ", GUILayout.ExpandHeight(true)))
                        {
                            fslots.RemoveAt(count);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndHorizontal();
                        count++;
                    }
                }

                Utils.PickerAction(OnPickerUpdate);
            }

            if (GUILayout.Button("Add New Feature"))
            {
                fslots.Add(new aiFeatureSlot("", null, false));
            }
        }

        Action OnPickerUpdate = () =>
        {
            var pickedObject = EditorGUIUtility.GetObjectPickerObject();
            string pickedObjPath = AssetDatabase.GetAssetPath(pickedObject);
            if (fslots != null && fslots.Count > 0)
            {
                fslots[slotIdx].feature = AssetDatabase.LoadAssetAtPath<aiFeature>(pickedObjPath);
            }
        };

        void CreateOrDelFeature()
        {
            fslots = ftMng.featureSlots;

            if (fslots != null && fslots.Count > 0)
            {
                CreateRoot();

                AddAllFeatureToRoot();

                slotCounting = ftMng.featureSlots.Count;

                for (int i = 0; i < slotCounting; i++)
                {
                    if (fslots[i].feature != null)
                    {
                        if (IsPrefab(fslots[i].feature))
                        {
                            var featureClone = Instantiate(fslots[i].feature, Vector3.zero, Quaternion.identity);
                            featureClone.name = fslots[i].name = "NewFeature";
                            featureClone.transform.SetParent(root);
                            featureClone.transform.localPosition = Vector3.zero;
                            featureClone.transform.localScale = Vector3.one;
                            featureClone.transform.rotation = Quaternion.identity;
                            var abScript = featureClone.GetComponent<aiFeature>();
                            if (abScript) abScript.id = Utils.uniqueID();
                            fslots[i].feature = abScript;
                        }
                        else
                        {
                            CreateRoot();

                            var featureClone = GetFeature(fslots[i].feature.id);

                            if (featureClone == null)
                            {
                                var newFeature = Instantiate(fslots[i].feature, Vector3.zero, Quaternion.identity);
                                newFeature.name = fslots[i].name = "NewFeature";
                                newFeature.transform.SetParent(root);

                                newFeature.transform.localPosition = Vector3.zero;
                                newFeature.transform.localScale = Vector3.one;
                                newFeature.transform.rotation = Quaternion.identity;

                                var abScript = newFeature.GetComponent<aiFeature>();
                                if (abScript) abScript.id = Utils.uniqueID();
                                fslots[i].feature = abScript;
                                
                            }
                            else
                            {
                                featureClone.name = fslots[i].name;
                            }
                        }
                    }
                }

                var featureIds = ftMng.GetFeatureIds();

                var featureCloneIds = GetAllFeatureCloneId();

                var featureIdsLeft = featureCloneIds.Except(featureIds).ToArray();

                if (featureIdsLeft != null && featureIdsLeft.Length > 0)
                {
                    for (int i = 0; i < featureIdsLeft.Length; i++)
                    {
                        var featureFinded = GetFeature(featureIdsLeft[i]);

                        if (featureFinded)
                        {

                            DestroyImmediate(featureFinded.gameObject, true);

                        }
                    }
                }
            }
            else
            {
                CreateRoot();
            }
        }

        void AddAllFeatureToRoot()
        {
            var abMngObj = ftMng.gameObject;

            if (abMngObj && root != null)
            {
                var abMngObjTrans = abMngObj.transform;

                if (abMngObjTrans)
                {
                    var maxChild = abMngObjTrans.childCount;
                    for (int i = 0; i < maxChild; i++)
                    {
                        var child = abMngObjTrans.GetChild(i);

                        if (child)
                        {
                            var featureClone = child.GetComponent<aiFeature>();

                            if (featureClone)
                            {
                                featureClone.transform.SetParent(root);
                                featureClone.transform.localPosition = Vector3.zero;
                                featureClone.transform.localScale = Vector3.one;
                                featureClone.transform.rotation = Quaternion.identity;
                            }
                        }
                        maxChild = abMngObjTrans.childCount;
                    }
                }
            }
        }

        List<string> GetAllFeatureCloneId()
        {
            List<string> ids = new List<string>();

            if (root)
            {
                var maxChild = root.childCount;

                for (int i = 0; i < maxChild; i++)
                {
                    var featureObj = root.GetChild(i);

                    if (featureObj)
                    {
                        var featureClone = featureObj.GetComponent<aiFeature>();

                        if (featureClone)
                        {
                            ids.Add(featureClone.id);
                        }
                    }
                }
            }
            return ids;
        }

        void CreateRoot()
        {
            var abMngObj = ftMng.gameObject;

            root = GetRoot();

            if (abMngObj && root == null)
            {
                var rootClone = new GameObject(rootName);

                rootClone.transform.SetParent(abMngObj.transform);
                rootClone.transform.localPosition = Vector3.zero;
                rootClone.transform.localScale = Vector3.one;
                rootClone.transform.rotation = Quaternion.identity;
            }
        }

        Transform GetRoot()
        {
            var abMngObj = ftMng.gameObject;

            if (abMngObj)
            {
                var abMngObjTrans = abMngObj.transform;

                var maxChild = abMngObjTrans.childCount;

                for (int i = 0; i < maxChild; i++)
                {
                    if (string.Compare(abMngObjTrans.GetChild(i).name, rootName) == 0)
                    {
                        return abMngObjTrans.GetChild(i);
                    }
                }
            }
            return null;
        }

        aiFeature GetFeature(string id)
        {
            if (root)
            {
                var maxChild = root.childCount;

                for (int i = 0; i < maxChild; i++)
                {
                    var featureObj = root.GetChild(i);

                    if (featureObj)
                    {
                        var featureClone = featureObj.GetComponent<aiFeature>();

                        if (featureClone && string.Compare(featureClone.id, id) == 0)
                        {
                            return featureClone;
                        }
                    }
                }
            }
            return null;
        }

        bool IsPrefab(UnityEngine.Object obj)
        {
            var root = PrefabUtility.GetNearestPrefabInstanceRoot(obj);

            if (root)
            {
                var status = PrefabUtility.GetPrefabInstanceStatus(root);

                return status != PrefabInstanceStatus.NotAPrefab && status != PrefabInstanceStatus.Connected ? false : true;
            }
            else
            {
                return obj != null && PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab;
            }
        }
    }
}