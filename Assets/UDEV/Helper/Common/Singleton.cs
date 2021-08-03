using UnityEngine;
using System.Collections;

// a Generic Singleton class

namespace UDEV
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public string sceneName;
        protected int numofEnterScene;

        // private static instance
        static T m_ins;

        // public static instance used to refer to Singleton (e.g. MyClass.Instance)
        public static T Ins
        {
            get
            {
                // if no instance is found, find the first GameObject of type T
                if (m_ins == null)
                {
                    m_ins = GameObject.FindObjectOfType<T>();

                    // if no instance exists in the Scene, create a new GameObject and add the Component T 
                    if (m_ins == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        m_ins = singleton.AddComponent<T>();
                    }
                }
                // return the singleton instance
                return m_ins;
            }
        }

        public virtual void Awake()
        {
            MakeSingleton(true);
        }

        public virtual void Start()
        {
            CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);

            numofEnterScene = CUtils.IncreaseNumofEnterScene(sceneName);

            CPlayerPrefs.Save();
            if (JobWorker.Ins.onEnterScene != null)
            {
                JobWorker.Ins.onEnterScene(sceneName);
            }

#if UNITY_WSA && !UNITY_EDITOR
        StartCoroutine(SavePrefs());
#endif
            //CUtils.ShowBannerAd();
        }

        public virtual void OnApplicationPause(bool pause)
        {
            Debug.Log("On Application Pause");
            CPlayerPrefs.Save();
        }

        private IEnumerator SavePrefs()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);
                CPlayerPrefs.Save();
            }
        }

        public void MakeSingleton(bool destroyOnload)
        {
            if (m_ins == null)
            {
                m_ins = this as T;
                if (destroyOnload)
                {
                    var root = transform.root;

                    if (root != transform)
                    {
                        DontDestroyOnLoad(root);
                    }
                    else
                    {
                        DontDestroyOnLoad(this.gameObject);
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}