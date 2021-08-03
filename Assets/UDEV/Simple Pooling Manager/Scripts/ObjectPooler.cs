using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object Pooler
/// - allows the reuse of frequently "spawned" objects for optimization
/// </summary>

namespace UDEV.SPM
{
    [CreateAssetMenu(fileName = "NEW_POOLER", menuName = "UDEV/SPM/Create Pooler")]
    public class ObjectPooler : ScriptableObject
    {
        [UniqueId]
        public string id;
        public PoolerTarget target;

        [System.Serializable]
        public class PoolerCategory
        {
            public string name;
            [UniqueId]
            public string id;


            public PoolerCategory() { }
            public PoolerCategory(string _name, string _id)
            {
                name = _name;
                id = _id;
            }
        }

        public List<PoolerCategory> categories = new List<PoolerCategory>();

        [System.Serializable]
        public class Pool
        {
            public string poolName;
            [UniqueId]
            public string id;
            [HideInInspector]
            public List<GameObject> pooledObjects;
            public GameObject prefab;
            public Transform startingParent;
            public int startingQuantity = 10;
            [PoolerCategory]
            public string category;

            public Pool(string _poolName, List<GameObject> _pooledObjects, GameObject _prefab, Transform _startingParent, int _startingQuantity, string _category)
            {
                poolName = _poolName;
                pooledObjects = _pooledObjects;
                prefab = _prefab;
                startingParent = _startingParent;
                startingQuantity = _startingQuantity;
                category = _category;
            }
        }
        public List<Pool> pools;

        // Use this for initialization
        void Start()
        {
            pools = new List<Pool>();

            ClearPooledObjects();

            for (int p = 0; p < pools.Count; p++)
            {
                for (int i = 0; i < pools[p].startingQuantity; i++)
                {
                    GameObject o = Instantiate(pools[p].prefab, Vector3.zero, Quaternion.identity, pools[p].startingParent ? pools[p].startingParent : null);
                    o.SetActive(false);
                    pools[p].pooledObjects.Add(o);
                }
            }
        }

        public GameObject Spawn(string poolId, Vector3 position, Quaternion rotation, Transform parentTransform = null)
        {
            if (IsPoolerExist(poolId))
            {
                // Find the pool that matches the pool name:
                int pool = 0;
                for (int i = 0; i < pools.Count; i++)
                {
                    if (pools[i].id == poolId)
                    {
                        pool = i;
                        break;
                    }
                    if (i == pools.Count - 1)
                    {
                        Debug.LogError("There's no pool named \"" + poolId + "\"! Check the spelling or add a new pool with that name.");
                        return null;
                    }
                }

                for (int i = 0; i < pools[pool].pooledObjects.Count; i++)
                {
                    if (pools[pool].pooledObjects[i] &&
                        !pools[pool].pooledObjects[i].activeSelf)
                    {
                        // Set active:
                        pools[pool].pooledObjects[i].SetActive(true);
                        pools[pool].pooledObjects[i].transform.localPosition = position;
                        pools[pool].pooledObjects[i].transform.localRotation = rotation;
                        // Set parent:
                        if (parentTransform)
                        {
                            pools[pool].pooledObjects[i].transform.SetParent(parentTransform, false);
                        }

                        return pools[pool].pooledObjects[i];
                    }
                }
                // If there's no game object available then expand the list by creating a new one:
                GameObject o = null;
                if(pools[pool].prefab)
                {
                    o = Instantiate(pools[pool].prefab, position, rotation);
                    pools[pool].pooledObjects.Add(o);
                }

                // Add newly instantiated object to pool:
                return o;
            }

            return null;
        }

        public void ClearPooledObjects()
        {
            foreach (Pool pool in pools)
            {
                pool.pooledObjects.Clear();
            }
        }

        public Dictionary<string, string> GetPoolIds()
        {
            Dictionary<string, string> ids = new Dictionary<string, string>();
            
            if(pools != null && pools.Count > 0)
            {
                for (int i = 0; i < pools.Count; i++)
                {
                    if (pools[i] != null)
                    {
                        if (!string.IsNullOrEmpty(pools[i].id))
                        {
                            ids[pools[i].id] = GetCategoryName(pools[i].category) + "/" + pools[i].poolName;
                        }
                    }
                }
            }

            return ids;
        }

        string GetCategoryName(string id)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i] != null && string.Compare(categories[i].id, id) == 0)
                {
                    return categories[i].name;
                }
            }

            return "";
        }

        bool IsPoolerExist(string id)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i] != null && string.Compare(pools[i].id, id) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

