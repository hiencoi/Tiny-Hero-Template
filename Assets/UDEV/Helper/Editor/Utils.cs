using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;

namespace UDEV
{
    public static class Utils
    {
        internal static int currentPickerWindow;

        public static void CreateMissingDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        public static string uniqueID()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
            int z1 = UnityEngine.Random.Range(0, 1000000);
            int z2 = UnityEngine.Random.Range(0, 1000000);
            string uid = currentEpochTime + "" + z1 + "" + z2;
            return uid;
        }

        public static void SaveDataToFile<T>(string path, string fileName, T data)
        {
            CreateMissingDirectory(path);
            FileStream fs = new FileStream(path + fileName, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, data);
            fs.Close();
        }

        public static T LoadDataFromFile<T>(string filePath, T data)
        {
            if (File.Exists(filePath))
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    var bformatter = new BinaryFormatter();

                    data = (T) bformatter.Deserialize(stream);

                    return data ;
                }
            }

            return default;
        }

        public static bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static List<GameObject> FindObjectInChilds(this GameObject gameObject, string tag)
        {
            List<GameObject> findeds = new List<GameObject>();

            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in children)
            {
                if (item.CompareTag(tag))
                {
                    findeds.Add(item.gameObject);
                }
            }

            return findeds;
        }

        public static void ShowPicker<T>(string filter) where T : UnityEngine.Object
        {
            // Create a window picker control ID
            currentPickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive);
            // Use the ID you just created
            EditorGUIUtility.ShowObjectPicker<T>(null, false, filter, currentPickerWindow);
        }

        public static void PickerAction(Action OnUpdate = null, Action OnClose = null)
        {
            // New object selected in picker window
            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
            {
                if (OnUpdate != null)
                    OnUpdate.Invoke();
            }
            // Picker window closed
            if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
            {
                if (OnClose != null)
                    OnClose.Invoke();
            }
        }
    }
}
