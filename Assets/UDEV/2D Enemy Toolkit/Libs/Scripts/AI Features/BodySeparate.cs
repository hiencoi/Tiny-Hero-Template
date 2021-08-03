using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.SPM;
using UDEV.TinyHero;

namespace UDEV.AI2D
{
    public class BodySeparate : aiFeature
    {
        SpawnWave m_wave;
        SpawnSpawner m_spawner;
        aiSpawned m_baseSpawnedComp;

        [System.Serializable]
        public class Body
        {
            public Vector3 spawnOffset;
            [PoolerKeys(target = PoolerTarget.AISPAWN)]
            public string bodyPool;
        }

        [Header("Feature Settings:")]
        public Body[] bodys;

        protected override void Init()
        {
            m_baseSpawnedComp = m_aiController.GetComponent<aiSpawned>();
            if (m_baseSpawnedComp)
            {
                m_baseSpawnedComp.onBeforeDead = () =>
                {
                    Separate();
                };
            }
        }

        void Separate()
        {
            m_wave = MissionsManager.Ins.MissionController.waves[m_baseSpawnedComp.waveIndex];
            if (m_wave != null)
            {
                m_spawner = m_wave.spawners[m_baseSpawnedComp.spawnerIndex];

                m_wave.spawnerEnemiesTotal += bodys.Length;

                if (m_spawner != null)
                    m_spawner.enemiesTotal += bodys.Length;
            }

            StartCoroutine(SeparateDelay());
        }

        IEnumerator SeparateDelay()
        {
            float delay = 0;

            var state = Helper.GetClip(m_aiController.anim, m_aiController.deathState.clipName);

            if(state)
                delay = state.length;

            yield return new WaitForSeconds(delay);

            if (bodys != null && bodys.Length > 0)
            {
                for (int i = 0; i < bodys.Length; i++)
                {
                    if (bodys[i] != null)
                    {
                        string bodyPool = bodys[i].bodyPool;

                        var aiSeparated = PoolersManager.Ins.Spawn(PoolerTarget.AISPAWN, bodyPool, m_aiController.transform.position, Quaternion.identity);

                        if (aiSeparated)
                        {
                            aiSeparated.transform.position = (m_aiController.transform.position) + bodys[i].spawnOffset;

                            var aiSpawnedComp = aiSeparated.GetComponent<aiSpawned>();

                            if (!aiSpawnedComp)
                            {
                                aiSpawnedComp = aiSeparated.AddComponent<aiSpawned>();
                            }

                            if (m_baseSpawnedComp)
                            {
                                aiSpawnedComp.msController = m_baseSpawnedComp.msController;
                                aiSpawnedComp.waveIndex = m_baseSpawnedComp.waveIndex;
                                aiSpawnedComp.spawnerIndex = m_baseSpawnedComp.spawnerIndex;
                            }
                        }
                    }
                }
            }
        }
    }
}
