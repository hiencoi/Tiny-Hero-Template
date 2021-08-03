using UnityEngine;
using System.Linq;

namespace UDEV.SPM
{
    public class PoolersManager : Singleton<PoolersManager>
    {
        public ObjectPooler[] objectPoolers;

        public GameObject Spawn(PoolerTarget target, string poolId, Vector3 position, Quaternion rotation, Transform parentTransform = null)
        {
            var poolers = objectPoolers.Where(t => t.target == target).ToArray();

            GameObject pool = null;

            if (poolers != null && poolers.Length > 0)
            {
                for (int i = 0; i < poolers.Length; i++)
                {
                    if (poolers[i] != null)
                        pool = poolers[i].Spawn(poolId, position, rotation, parentTransform);

                    if (pool) return pool;
                }
            }
            else
            {
                Debug.LogWarning("Pooler for " + target.ToString() + " is Null.Please add it to Pooler Manager!.");
            }

            if (pool == null)
            {
                Debug.LogWarning("Pool key does not exist in pooler for " + target.ToString());
            }

            return pool;
        }

        public void Clear(PoolerTarget target)
        {
            var poolers = objectPoolers.Where(t => t.target == target).ToArray();

            if (poolers != null && poolers.Length > 0)
            {
                for (int i = 0; i < poolers.Length; i++)
                {
                    if (poolers[i] != null)
                        poolers[i].ClearPooledObjects();
                }
            }
        }

        public void ClearAll()
        {
            if (objectPoolers != null && objectPoolers.Length > 0)
            {
                for (int i = 0; i < objectPoolers.Length; i++)
                {
                    if (objectPoolers[i] != null)
                    {
                        objectPoolers[i].ClearPooledObjects();
                    }
                }
            }
        }
    }
}
