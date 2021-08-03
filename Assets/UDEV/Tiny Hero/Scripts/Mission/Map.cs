using UnityEngine;

namespace UDEV.TinyHero
{
    public class Map : MonoBehaviour
    {
        public Transform playerSpawnPoint;

        public Transform[] enemySpawnPoints;

        public Transform GetEnemySpawnPoint(Transform point)
        {
            if(enemySpawnPoints != null && enemySpawnPoints.Length > 0)
            {
                for (int i = 0; i < enemySpawnPoints.Length; i++)
                {
                    if (enemySpawnPoints[i] != null && enemySpawnPoints[i] == point)
                        return enemySpawnPoints[i];
                }
            }
            return null;
        }
    }
}
