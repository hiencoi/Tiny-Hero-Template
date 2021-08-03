using UnityEngine;
using UDEV.SPM;
using System.Linq;

namespace UDEV.TinyHero
{
    public class CollectablesManager : Singleton<CollectablesManager>
    {
        public float explodeForce;

        public Collectable[] commons;
        public Collectable[] rares;
        public Collectable[] epics;

        /// <summary>
        /// Spawn Collectable
        /// </summary>
        /// <param name="collectables">Collectable Pooled</param>
        /// <param name="position">Spawn Position</param>
        /// <param name="amount">Quanlity</param>
        void SpawnCollectable(Collectable[] collectables, Vector3 position, int amount = 1)
        {
            if (collectables != null && collectables.Length > 0)
            {
                float check = Random.Range(0f, 1f);

                Collectable[] c = collectables.Where(w => w.rate >= check).ToArray();

                if (c.Length <= 0) return;

                for (int i = 0; i < amount; i++)
                {
                    var randomIdx = Random.Range(0, c.Length);
                    var cClone = PoolersManager.Ins.Spawn(PoolerTarget.COLLECTABLE, c[randomIdx].pooled, position, Quaternion.identity);
                    if (cClone)
                        cClone.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-explodeForce, explodeForce), Random.Range(-explodeForce, explodeForce));
                }
            }
        }

        /// <summary>
        /// Spawn Common Collectables
        /// </summary>
        /// <param name="pos">Spawn Position</param>
        /// <param name="amount">Quanlity</param>
        public void SpawnCommonCollectable(Vector3 pos, int amount = 1)
        {
            SpawnCollectable(commons, pos, amount);
        }

        /// <summary>
        /// Spawn Rare Collectables
        /// </summary>
        /// <param name="pos">Spawn Position</param>
        /// <param name="amount">Quanlity</param>
        public void SpawnRareCollectable(Vector3 pos, int amount = 1)
        {
            SpawnCollectable(rares, pos, amount);
        }

        /// <summary>
        /// Spawn Epic Collectables
        /// </summary>
        /// <param name="pos">Spawn Position</param>
        /// <param name="quanlity">Quanlity</param>
        public void SpawnEpicCollectable(Vector3 pos, int quanlity = 1)
        {
            SpawnCollectable(epics, pos, quanlity);
        }
    }

    [System.Serializable]
    public class Collectable
    {
        [Range(0f, 1f)]
        public float rate = 1f;
        [PoolerKeys(target = PoolerTarget.COLLECTABLE)]
        public string pooled;
    }
}
