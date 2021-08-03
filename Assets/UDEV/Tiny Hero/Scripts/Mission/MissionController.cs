using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UDEV.SPM;

namespace UDEV.TinyHero {
    public class MissionController : MonoBehaviour
    {
        [UniqueId]
        public string id;

        public Map map;
        Map m_curMap;

        // the type of events
        public enum SpawnStates { BeforeWave, BetweenWaves, InWave, MissionComplete }

        public SpawnStates spawnState; // static, the current spawn state we are in
        public int wavesLength; // holds waves.Count
        public bool forcedWave; // static, if true will force a ResetWave on the currentWave
        public int currentWave; // static, the current wave we are in

        public int playWave = 0; // the init play wave we want to start at
        public bool playOnStart = true; // play the waves at the start?
        public int restart = 0; // restart the waves? 0 - no restart, (n - 1) the wave to restart at
        public float timeBeforeFirstWave = 2f; // the amount of time before the first wave
        public float timeBetweenWaves = 4f; // the amount of time between waves
        public List<SpawnWave> waves = new List<SpawnWave>();

        private float timer = 0f; // internal timer
        private float deltaTime; // alias of Time.deltaTime
        private bool isPaused = true; // if paused (be default) we stop running, otherwise we spawn enemies
        private List<Hashtable> events = new List<Hashtable>(); // holds the events created by ther user

        public Map CurMap { get => m_curMap;}

        private void Awake()
        {
            m_curMap = Instantiate(map, map.transform.position, Quaternion.identity);
        }

        public void Start()
        {
            wavesLength = waves.Count; // keep track of the number of waves
            PlayWave(playWave); // the wave we want to play
            isPaused = !playOnStart; // ditto

            // now lets use the pool manager, we need to create a pmp for EACH enemy with a unique name
            foreach (SpawnWave wave in waves)
            {
                foreach (SpawnSpawner spawner in wave.spawners)
                {
                    foreach (SpawnEnemy enemy in spawner.enemies)
                    {
                        int count = enemy.isRandom ? enemy.objects.Count : 1;

                        for (int i = 0; i < count; i++)
                        {

                        }
                    } // end for enemy
                } // end for spawner
            } // end for wave
        }

        public void Update()
        {
            // if this was a forced wave (was set via NextWave, PreviousWave, or PlayWave), lets make sure it's been resetted
            if (forcedWave)
            {
                ResetWave(currentWave);
                forcedWave = false;
            }

            // wha?
            if (wavesLength == 0)
                return;

            // hold up, don't move
            if (isPaused)
                return;

            deltaTime = Time.deltaTime;

            // wait a bit before we start our first wave
            if (spawnState == SpawnStates.BeforeWave)
            {
                timer += deltaTime;

                if (timer >= timeBeforeFirstWave)
                {
                    timer = 0f;
                    spawnState = SpawnStates.InWave; // set the state
                    EventInvoke(SpawnStates.InWave); // invoke the event
                }
            }

            // we are in the middle of the waves
            if (spawnState == SpawnStates.BetweenWaves)
            {
                timer += deltaTime;

                if (timer >= timeBetweenWaves)
                {
                    timer = 0f;
                    spawnState = SpawnStates.InWave; // set the state
                    EventInvoke(SpawnStates.InWave); // invoke the event
                }
            }

            // in the wave, start the spawning
            if (spawnState == SpawnStates.InWave)
            {
                // grab the wave
                SpawnWave wave = waves[currentWave] as SpawnWave;

                // timed
                if (wave.type == 0)
                {
                    // we don't need to decrement the length if we allow the slow pokes
                    if (wave.stranglers)
                    {
                        spawnEnemies(wave);

                        if (areSpawnersComplete(wave))
                            waveCompleted();
                    }
                    else
                    {
                        // spawn the enemies until we are out of time
                        wave.length -= deltaTime;
                        if (wave.length >= 0)
                            spawnEnemies(wave);
                        // out of time, move onto the next wave... the spawners should not of had a Kill type
                        else
                            waveCompleted();
                        /*
                        {
                            if ( areSpawnersComplete(wave) )
                                waveCompleted();
                        }
                        */
                    }
                }
                // kill
                else if (wave.type == 1)
                {
                    //Debug.Log(wave.spawnerEnemiesKilled + " - " + wave.spawnerEnemiesTotal);
                    // killed all the spawned enemies in this wave, nice
                    if (wave.spawnerEnemiesKilled == wave.spawnerEnemiesTotal)
                    {
                        if (areSpawnersComplete(wave))
                            waveCompleted();
                    }
                    else
                        spawnEnemies(wave);
                }
            }
            
        }

        public void ResetWaves()
        {
            currentWave = 0;
            for (int i = 0; i < wavesLength; i++)
                ResetWave(i);

        }

        public void ResetWave(int index)
        {
            SpawnWave wave = waves[index] as SpawnWave;

            for (int i = 0; i < wave.spawners.Count; i++)
                ResetSpawner(wave, i);

            wave.reset();

        }

        public void ResetSpawner(SpawnWave wave, int index)
        {
            SpawnSpawner spawner = wave.spawners[index] as SpawnSpawner;

            foreach (SpawnEnemy enemy in spawner.enemies)
                enemy.reset();

            spawner.reset();
        }

        private bool areSpawnersComplete(SpawnWave wave)
        {
            foreach (SpawnSpawner spawner in wave.spawners)
            {
                if (!spawner.complete)
                    return false;
            }
            return true;
        }

        private void spawnEnemies(SpawnWave wave)
        {

            for (int i = 0; i < wave.spawners.Count; i++)
            {
                SpawnSpawner spawner = wave.spawners[i] as SpawnSpawner;

                // no need to continue, we are complete with this spawner
                if (spawner.complete)
                    continue;

                // we wait a bit
                if (spawner.initDelay >= 0.0)
                {
                    spawner.initDelay -= deltaTime;
                    continue;
                }

                spawner.rate -= deltaTime;

                // it is time!
                if (spawner.rate <= 0.0)
                {
                    GameObject go = null;
                    // if timed add the enemy, if kill and not spawned add enemy
                    if (spawner.type == 0 || (spawner.type == 1 && !spawner.spawned))
                    {
                        // get the enemy we will be adding
                        SpawnEnemy enemy = spawner.enemies[spawner.enemyIndex] as SpawnEnemy;

                        // if kill type we set the properties
                        if (spawner.type == 1)
                            spawner.spawned = true;

                        // add the enemy to the scene
                        var randomEnemyIdx = Random.Range(0, enemy.objects.Count);
                        string randomEnemyPool = enemy.objects[enemy.isRandom ? randomEnemyIdx : 0];

                        Vector3 spawnPoint = Vector3.zero;

                        spawnPoint = spawner.isRandom ? 
                            m_curMap.enemySpawnPoints[spawner.spawnPointIdxs[Random.Range(0, spawner.spawnPointIdxs.Count)]].position 
                            : m_curMap.enemySpawnPoints[spawner.spawnPointIdxs[0]].position;

                        go = PoolersManager.Ins.Spawn(PoolerTarget.AISPAWN ,randomEnemyPool, spawnPoint, Quaternion.identity);
                        if (go != null)
                        {
                            // attach our spawn enemy component, if doesn't exist
                            aiSpawned se = go.GetComponent<aiSpawned>() as aiSpawned;
                            if (!se)
                                se = go.AddComponent<aiSpawned>() as aiSpawned;

                            se.msController = this;
                            se.waveIndex = currentWave;
                            se.spawnerIndex = i;

                            enemy.repeat--;

                            // send messages?
                            for (int j = 0; j < enemy.spawnMessage.Count; j++)
                            {
                                string sm = enemy.spawnMessage[j].message as string;
                                string smv = enemy.spawnMessage[j].parameter as string;

                                if (sm != "")
                                    go.SendMessage(sm, smv, SendMessageOptions.DontRequireReceiver);
                            }
                            if (enemy.repeat <= 0)
                            {
                                // next enemy
                                spawner.enemyIndex++;

                                // if this is a timed type, check if we are done
                                if (spawner.type == 0 && spawner.enemyIndex == spawner.enemies.Count)
                                {
                                    if (spawner.endless)
                                        ResetSpawner(wave, i);
                                    else
                                        spawner.complete = true;
                                }
                            }
                        }
                    }

                    // reset the spawner rate
                    spawner.rate = spawner._rate;
                }
            } // end spawners loop
        }

        private void waveCompleted()
        {

            if (currentWave < wavesLength - 1)
            {
                NextWave();
                spawnState = SpawnStates.BetweenWaves; // set the state
                EventInvoke(SpawnStates.BetweenWaves); // invoke the event
            }
            else
            {
                if (restart > 0)
                    PlayWave(restart - 1);
                else
                {
                    spawnState = SpawnStates.MissionComplete; // set the state
                    EventInvoke(SpawnStates.MissionComplete); // invoke the event
                }
            }
        }

        public void PauseWave() { isPaused = true; }
        public void ResumeWave() { isPaused = false; }
        public void NextWave()
        {
            if (currentWave < wavesLength - 1)
            {
                currentWave++;
                forcedWave = true;
            }

        }
        public void PreviousWave()
        {
            if (currentWave > 0)
            {
                currentWave--;
                forcedWave = true;
            }
        }
        public void PlayWave(int index)
        {
            if (index <= wavesLength - 1 && index >= 0)
            {
                currentWave = index;
                forcedWave = true;

                spawnState = SpawnStates.BeforeWave; // set the state
                EventInvoke(SpawnStates.BeforeWave); // invoke the event
            }
        }

        public delegate void DelegateFtn();

        // adds an event
        public void AddEvent(SpawnStates state, DelegateFtn callback)
        {
            Hashtable e = new Hashtable();
            e.Add("state", state);
            e.Add("callback", callback);
            events.Add(e);
        }

        // removes an event
        public void EventRemove(SpawnStates state)
        {
            for (int i = 0; i < events.Count; i++)
            {
                Hashtable e = events[i] as Hashtable;
                if (e["state"].ToString() == state.ToString())
                    events.RemoveAt(i);
            }
        }

        // calls the event (private)
        private void EventInvoke(SpawnStates state)
        {
            for (int i = 0; i < events.Count; i++)
            {
                Hashtable e = events[i] as Hashtable;
                if (e["state"].ToString() == state.ToString())
                    (e["callback"] as DelegateFtn)();
            }
        }
    }

    [System.Serializable]
    public class SpawnWave
    {
        public int type = 0; // timed, kill
        public float length = 10f; // if timed, the number of seconds this wave will last. Used for timed type (0)
        public float _length; // holder
        public bool stranglers = true; // allow stranglers if timed type runs out of time?
        public List<SpawnSpawner> spawners = new List<SpawnSpawner>();
        public int spawnerEnemiesTotal = 0; // for kill type (1), keep track of the enemies spawned
        public int spawnerEnemiesKilled = 0; // for kill type (1), keep track of the enemies killed in the spawn
        public bool isBossWave;

        public void reset()
        {
            length = _length;

            // count the number of enemies this wave will have to kill
            spawnerEnemiesTotal = 0;
            spawnerEnemiesKilled = 0;
            for (int i = 0; i < spawners.Count; i++)
                spawnerEnemiesTotal += (spawners[i] as SpawnSpawner).enemiesTotal;
        }
    }

    [System.Serializable]
    public class SpawnSpawner
    {
        public int type = 0; // timed, kill
        public bool complete = false; // determines if this spawner is complete
        public bool endless = false; // determines if this spawner ever ends
        public float rate = 1f; // if timed, the rate which the enemy spawns - if kill, the time after the previous enemy dies
        public float _rate; // holder
        public bool isRandom = false; // want to spawn from random spawners?
        public List<int> spawnPointIdxs = new List<int>(); // the random spawners our enemies will spawn from (if isRandom is true)
        public float initDelay = 0f; // initial delay when starting the spawn
        public float _initDelay; // holder
        public int enemyIndex = 0; // the enemy index we are looping through
        public int _enemyIndex; // holder
        public List<SpawnEnemy> enemies = new List<SpawnEnemy>();
        public int enemiesTotal = 0; // for kill type (1), we count the number of enemies this spawner will have
        public int enemiesKilled = 0; // for kill type (1), keep track the number of enemies killed
        public bool spawned = false; // for kill type (1), so we know when to spawn

        public void reset()
        {
            complete = false;
            rate = _rate;
            initDelay = _initDelay;
            enemyIndex = 0;

            // count the number of enemies this spawner is to kill
            enemiesTotal = 0;
            enemiesKilled = 0;
            for (int i = 0; i < enemies.Count; i++)
                enemiesTotal += (enemies[i] as SpawnEnemy).repeat;
        }
    }

    [System.Serializable]
    public class SpawnEnemy
    {
        public int repeat = 1; // the number of times to repeat this enemy, once it reaches zero it moves onto the next enemy in the spawner enemies index
        public int _repeat; // holder
        public bool isRandom = false; // want to spawn random enemies
        [PoolerKeys(target = PoolerTarget.AISPAWN)]
        public List<string> objects = new List<string>(); // the random enemies to spawn (if isRandom is true)
        public List<SpawnMessage> spawnMessage = new List<SpawnMessage>(); // when intiating the enemy, the option val SendMessage

        public void reset()
        {
            repeat = _repeat;
        }
    }

    [System.Serializable]
    public class SpawnMessage
    {
        public string message;
        public string parameter;

        public SpawnMessage(string _mess, string _param)
        {
            message = _mess;
            parameter = _param;
        }
    }
}