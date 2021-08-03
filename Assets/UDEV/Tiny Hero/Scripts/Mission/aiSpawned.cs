using UnityEngine;
using System.Collections.Generic;
using UDEV.AI2D;
using System;

namespace UDEV.TinyHero {
    public class aiSpawned : MonoBehaviour
    {
        public MissionController msController;
        public int waveIndex;
        public int spawnerIndex;
        [HideInInspector]
        public SpawnWave wave;
        [HideInInspector]
        public SpawnSpawner spawner;
        public Action onBeforeDead;

        private void Start()
        {
            msController = MissionsManager.Ins.MissionController;

            if (msController)
            {
                wave = msController.waves[waveIndex] as SpawnWave;
                spawner = wave.spawners[spawnerIndex] as SpawnSpawner;
            }
        }

        private void Update()
        {
            if (msController != null)
            {
                wave = msController.waves[waveIndex] as SpawnWave;
                spawner = wave.spawners[spawnerIndex] as SpawnSpawner;
            }
        }

        public void Dead()
        {
            if (onBeforeDead != null)
                onBeforeDead.Invoke();

            if(wave != null)
                wave.spawnerEnemiesKilled++;

            // kill type
            if (spawner.type == 1)
            {
                spawner.enemiesKilled++;
                if (spawner.enemiesKilled == spawner.enemiesTotal)
                {
                    if (spawner.endless)
                        msController.ResetSpawner(wave, spawnerIndex);
                    else
                        spawner.complete = true;
                }
                else
                {
                    spawner.spawned = false;
                    spawner.rate = spawner._rate;
                }
            }
        }
    }
}