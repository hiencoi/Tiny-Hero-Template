using UnityEditor;
using UnityEngine;
using System;

namespace UDEV.TinyHero {
    [CustomEditor(typeof(MissionController))]
    public class MissionControllerEditor : Editor
    {
        private MissionController sc = null;
        SerializedProperty m_idProperty;
        internal static SerializedProperty m_mapProp;

        public override void OnInspectorGUI()
        {
            sc = target as MissionController;
            
            m_idProperty = serializedObject.FindProperty("id");
            m_mapProp = serializedObject.FindProperty("map");

            if (sc == null)
                return;

            if (Application.isPlaying)
            {
                // current wave
                EditorGUILayout.LabelField("Current Wave", (sc.currentWave + 1).ToString());
                return;
            }

            // some space
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_idProperty, new GUIContent());

            EditorGUILayout.LabelField("Common Settings:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_mapProp, new GUIContent("Map"));

            if (GUILayout.Button("Select Prefab"))
            {
                Utils.ShowPicker<GameObject>("l:Map");
            }

            Utils.PickerAction(OnPickerUpdate);

            EditorGUILayout.EndHorizontal();

            // play wave
            GUIContent[] playWaves = new GUIContent[sc.waves.Count];
            for (int i = 0; i < sc.waves.Count; i++)
                playWaves[i] = new GUIContent("Wave " + (i + 1).ToString());

            // play wave
            sc.playWave = EditorGUILayout.Popup(new GUIContent("Play Wave", "Plays this wave once initiated"), sc.playWave, playWaves);

            // play on start
            sc.playOnStart = EditorGUILayout.Toggle(new GUIContent("Play On Start", "Plays the waves once the Scene is ready to go (other wise use SpawnController.ResumeWave() when ready)"), sc.playOnStart);

            // restart wave
            GUIContent[] restartWaves = new GUIContent[sc.waves.Count + 1];
            restartWaves[0] = new GUIContent("Do Not Restart");
            for (int i = 0; i < sc.waves.Count; i++)
                restartWaves[i + 1] = new GUIContent("Wave " + (i + 1).ToString());

            sc.restart = EditorGUILayout.Popup(new GUIContent("Restart Wave At", "Once the last wave has finished"), sc.restart, restartWaves);

            // time before first wave
            sc.timeBeforeFirstWave = EditorGUILayout.FloatField(new GUIContent("Time Before First", "The amount of time before we start this wave"), sc.timeBeforeFirstWave);

            // time between waves
            sc.timeBetweenWaves = EditorGUILayout.FloatField(new GUIContent("Time Between Waves", "The amount of time between waves"), sc.timeBetweenWaves);

            // some space
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Main of Wave Settings:", EditorStyles.boldLabel);

            // add/remove waves
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Wave"))
            {
                sc.waves.Add(new SpawnWave());
                State.configWave = sc.waves.Count - 1;
            }
            if (GUILayout.Button("Remove Wave") && sc.waves.Count > 0)
            {
                sc.waves.RemoveAt(State.configWave);
                State.configWave = sc.waves.Count - 1;
                State.configWave = Mathf.Clamp(State.configWave, 0, sc.waves.Count - 1);
            }
            GUILayout.EndHorizontal();

            // don't continue editing
            if (sc.waves.Count > 0)
            {
                State.configWave = Mathf.Clamp(State.configWave, 0, sc.waves.Count - 1);
                SerializedProperty waveProp = serializedObject.FindProperty("waves");
                SerializedProperty curWaveProp = null;
                if (waveProp != null && waveProp.arraySize > 0 && waveProp.arraySize == sc.waves.Count)
                    curWaveProp = waveProp.GetArrayElementAtIndex(State.configWave);

                if (curWaveProp != null)
                {
                    // list the waves
                    string[] numWaves = new string[sc.waves.Count];
                    for (int i = 0; i < sc.waves.Count; i++)
                        numWaves[i] = "Wave " + (i + 1).ToString() + " (of " + sc.waves.Count.ToString() + ")";

                    int bConfigWave = State.configWave;
                    State.configWave = EditorGUILayout.Popup("Config Wave", State.configWave, numWaves);

                    if (bConfigWave != State.configWave)
                    {
                        State.configSpawner = 0;
                        State.configEnemy = 0;
                    }

                    if (State.configWave > sc.waves.Count - 1)
                        State.configWave = 0;

                    // the wave we are editing
                    SpawnWave wave = sc.waves[State.configWave] as SpawnWave;

                    // wave type
                    GUIContent[] waveTypes = new GUIContent[2];
                    waveTypes[0] = new GUIContent("Time");
                    waveTypes[1] = new GUIContent("Kill");
                    wave.type = EditorGUILayout.Popup(new GUIContent("Type", "Time: spawned enemies until the desired Time has been reached\nKill: all spawned enemies must be killed before proceeding onto the next wave"), wave.type, waveTypes);

                    // show options depending on type
                    if (wave.type == 0) // show Timed options
                    {
                        wave.length = EditorGUILayout.FloatField(new GUIContent("Time", "How long this wave will run"), wave.length);
                        wave._length = wave.length;

                        bool stranglers = false;
                        for (int i = 0; i < wave.spawners.Count; i++)
                            if ((wave.spawners[i] as SpawnSpawner).type == 1)
                                stranglers = true;

                        // they don't have a choice
                        if (stranglers)
                        {
                            EditorGUILayout.Toggle(new GUIContent("Allow Stranglers", "Will continue to spawnn enemies if Time has succeeded (Note: this will be checked if any of it's spawners have a Kill Type"), true);
                            wave.stranglers = true;
                        }
                        // they have a choice
                        else
                            wave.stranglers = EditorGUILayout.Toggle(new GUIContent("Allow Stranglers", "Will continue to spawnn enemies if Time has succeeded (Note: this will be checked if any of it's spawners have a Kill Type"), wave.stranglers);
                    }

                    sc.waves[State.configWave].isBossWave = EditorGUILayout.Toggle("Is Boss Wave", sc.waves[State.configWave].isBossWave);

                    // some space
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Spawners of Wave Settings:", EditorStyles.boldLabel);
                    // add/remove spawners
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Spawner"))
                    {
                        wave.spawners.Add(new SpawnSpawner());
                        State.configSpawner = wave.spawners.Count - 1;
                    }
                    if (GUILayout.Button("Remove Spawner") && wave.spawners.Count > 0)
                    {
                        wave.spawners.RemoveAt(State.configSpawner);
                        State.configSpawner = wave.spawners.Count - 1;
                    }
                    GUILayout.EndHorizontal();

                    // don't continue editing
                    if (wave.spawners.Count > 0)
                    {
                        // show the spawners in the wave
                        string[] listSpawners = new string[wave.spawners.Count];
                        for (int i = 0; i < wave.spawners.Count; i++)
                            listSpawners[i] = "Spawner " + (i + 1).ToString() + " (of " + wave.spawners.Count.ToString() + ")";

                        int bConfigSpawner = State.configSpawner;
                        State.configSpawner = EditorGUILayout.Popup("Config Spawner", State.configSpawner, listSpawners);

                        if (bConfigSpawner != State.configSpawner)
                            State.configEnemy = 0;

                        if (State.configSpawner > wave.spawners.Count - 1)
                            State.configSpawner = 0;

                        // the spawner we are editing
                        SpawnSpawner spawner = wave.spawners[State.configSpawner] as SpawnSpawner;

                        SerializedProperty spawnerProp = null;

                        if (spawner != null && curWaveProp != null)
                            spawnerProp = curWaveProp.FindPropertyRelative("spawners");
                        SerializedProperty curSpawnerProp = null;

                        if (spawnerProp != null && spawnerProp.arraySize == wave.spawners.Count)
                            curSpawnerProp = spawnerProp.GetArrayElementAtIndex(State.configSpawner);
                        // spawner type
                        GUIContent[] spawnerTypes = new GUIContent[2];
                        spawnerTypes[0] = new GUIContent("Time");
                        spawnerTypes[1] = new GUIContent("Kill");
                        spawner.type = EditorGUILayout.Popup(new GUIContent("Type", "Timed: spawns enemies at the set Rate\nKill: spawned enemy must be killed in order to proceed onto the next enemy"), spawner.type, spawnerTypes);

                        // endless
                        spawner.endless = EditorGUILayout.Toggle(new GUIContent("Endless", "Will continuously spawn enemies until stopped by a script"), spawner.endless);

                        // show options depending on type
                        if (spawner.type == 0) // show Timed options
                        {
                            spawner.rate = EditorGUILayout.FloatField(new GUIContent("Rate", "The timed rate at which enemies will be spawned"), spawner.rate);
                            spawner._rate = spawner.rate;
                            spawner.initDelay = EditorGUILayout.FloatField(new GUIContent("Initial Delay", "The amount of time before the first enemy is spawned"), spawner.initDelay);
                            spawner._initDelay = spawner.initDelay;
                        }
                        else if (spawner.type == 1) // show Kill options
                        {
                            spawner.rate = EditorGUILayout.FloatField(new GUIContent("Delay Between Spawns", "The amount of time between spawns"), spawner.rate);
                            spawner._rate = spawner.rate;
                        }

                        // list the spawners
                        int spawnerSelected;

                        spawner.isRandom = EditorGUILayout.Toggle(new GUIContent("Random Spawners?", "If you want the enemies to spawn at random Spawners"), spawner.isRandom);

                        int count = spawner.isRandom ? spawner.spawnPointIdxs.Count : 1;

                        // fix
                        if (spawner.spawnPointIdxs.Count == 0)
                            spawner.spawnPointIdxs.Add(spawner.spawnPointIdxs.Count);


                        EditorGUILayout.LabelField("List of Spawners:");

                        if (sc.map && sc.map.enemySpawnPoints != null && sc.map.enemySpawnPoints.Length > 0)
                        {
                            Transform[] enemySpawnPoints = sc.map.enemySpawnPoints;

                            GUIContent[] spawnerNames = new GUIContent[enemySpawnPoints.Length];

                            for (int i = 0; i < enemySpawnPoints.Length; i++)
                                if (enemySpawnPoints[i] != null)
                                    spawnerNames[i] = new GUIContent(enemySpawnPoints[i].name);

                            for (int i = 0; i < count; i++)
                            {
                                spawnerSelected = 0;

                                for (int j = 0; j < enemySpawnPoints.Length; j++)
                                    spawnerSelected = spawner.spawnPointIdxs[i] == j ? j : spawnerSelected;

                                GUILayout.BeginHorizontal();

                                EditorGUILayout.LabelField(i == 0 ? "Spawner" : "");
                                spawner.spawnPointIdxs[i] = EditorGUILayout.Popup(spawnerSelected, spawnerNames);

                                if (spawner.isRandom && count > 1)
                                {
                                    if (GUILayout.Button("-"))
                                    {
                                        spawner.spawnPointIdxs.RemoveAt(i);
                                        break;
                                    }
                                }

                                GUILayout.EndHorizontal();
                            }

                            // only show if it was random
                            if (spawner.isRandom)
                            {
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("");
                                if (GUILayout.Button("Add Random Spawner"))
                                {
                                    if (enemySpawnPoints != null && enemySpawnPoints.Length > 0)
                                    {
                                        if (sc.map.enemySpawnPoints[0] != null)
                                        {
                                            spawner.spawnPointIdxs.Add(spawner.spawnPointIdxs.Count);
                                        }
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Please add a map!.", MessageType.Error);
                        }
                        // some space
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Enemies of Wave Settings:", EditorStyles.boldLabel);
                        // add/remove enemies
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Enemy"))
                        {
                            spawner.enemies.Add(new SpawnEnemy());

                            State.configEnemy = spawner.enemies.Count - 1;
                        }
                        if (GUILayout.Button("Remove Enemy") && spawner.enemies.Count > 0)
                        {
                            spawner.enemies.RemoveAt(State.configEnemy);
                            State.configEnemy = spawner.enemies.Count - 1;
                        }
                        GUILayout.EndHorizontal();

                        SerializedProperty enemyProp = null;
                        if (curSpawnerProp != null)
                            enemyProp = curSpawnerProp.FindPropertyRelative("enemies");
                        State.configEnemy = Mathf.Clamp(State.configEnemy, 0, spawner.enemies.Count - 1);
                        SerializedProperty curEnemyProp = null;
                        if ((enemyProp != null) && enemyProp.arraySize == spawner.enemies.Count && enemyProp.arraySize > 0)
                            curEnemyProp = enemyProp.GetArrayElementAtIndex(State.configEnemy);

                        // don't continue editing
                        if (spawner.enemies.Count > 0)
                        {
                            // make the list of enemies
                            string[] enemyList = new string[spawner.enemies.Count];
                            for (int i = 0; i < spawner.enemies.Count; i++)
                                enemyList[i] = "Enemy " + (i + 1).ToString() + " (of " + spawner.enemies.Count.ToString() + ")";

                            State.configEnemy = EditorGUILayout.Popup("Config Enemy", State.configEnemy, enemyList);

                            if (State.configEnemy > spawner.enemies.Count - 1)
                                State.configEnemy = 0;


                            // the enemy we are editing
                            SpawnEnemy enemy = spawner.enemies[State.configEnemy] as SpawnEnemy;

                            // enemy repeat
                            enemy.repeat = EditorGUILayout.IntField(new GUIContent("Repeat", "The number of times to repeat this enemy"), enemy.repeat);
                            enemy._repeat = enemy.repeat;

                            // quick fix
                            if (enemy.objects.Count == 0)
                                enemy.objects.Add(null);

                            // random
                            enemy.isRandom = EditorGUILayout.Toggle(new GUIContent("Random Enemies?", "If you want the enemies to spawn to be random"), enemy.isRandom);

                            // count var is already instantiated from above
                            count = enemy.isRandom ? enemy.objects.Count : 1;

                            SerializedProperty enemyObjectsProp = null;

                            if (curEnemyProp != null)
                                enemyObjectsProp = curEnemyProp.FindPropertyRelative("objects");

                            // show the enemy
                            for (int i = 0; i < count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(i == 0 ? "Enemy" : "");

                                if (enemyObjectsProp != null && enemyObjectsProp.arraySize >= count)
                                    EditorGUILayout.PropertyField(enemyObjectsProp.GetArrayElementAtIndex(i), new GUIContent(""));

                                // only show if it was random
                                if (enemy.isRandom && count > 1)
                                {
                                    if (GUILayout.Button("-"))
                                    {
                                        enemy.objects.RemoveAt(i);
                                        break;
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }

                            // only show if it was random
                            if (enemy.isRandom)
                            {
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("");
                                if (GUILayout.Button("Add Random Enemy"))
                                    enemy.objects.Add(null);
                                GUILayout.EndHorizontal();
                            }

                            // some space
                            EditorGUILayout.Space();

                            // enemy send message(s)
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Send Messages (" + enemy.spawnMessage.Count.ToString() + ")");
                            if (GUILayout.Button("Add"))
                            {
                                enemy.spawnMessage.Add(new SpawnMessage("", ""));
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label(new GUIContent("Method", "The name of the function to execute"), GUILayout.Width(120));
                            GUILayout.Label(new GUIContent("Value", "The value to pass to the function (will be passed as a String)"), GUILayout.Width(120));
                            GUILayout.Label(" ");
                            EditorGUILayout.EndHorizontal();

                            string[] methods = new string[] { "HealthUp", "DamageUp", "BonusesUp" };

                            int methodSelected = 0;

                            for (int i = 0; i < enemy.spawnMessage.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();

                                methodSelected = Array.IndexOf(methods, enemy.spawnMessage[i].message);
                                methodSelected = methodSelected < 0 ? 0 : methodSelected;
                                methodSelected = EditorGUILayout.Popup(methodSelected, methods);
                                enemy.spawnMessage[i].message = methods[methodSelected];

                                enemy.spawnMessage[i].parameter = GUILayout.TextField(enemy.spawnMessage[i].parameter, GUILayout.Width(120));
                                if (GUILayout.Button("Remove"))
                                {
                                    enemy.spawnMessage.RemoveAt(i);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        Action OnPickerUpdate = () =>
        {
            var pickedObject = EditorGUIUtility.GetObjectPickerObject();
            string pickedObjPath = AssetDatabase.GetAssetPath(pickedObject);
            m_mapProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Map>(pickedObjPath);
        };

        public void OnSceneGUI()
        {
            if (Application.isPlaying)
                return;

            if (sc == null)
                return;

            if (sc.waves.Count == 0)
                return;

            // the wave we are editing
            SpawnWave wave = sc.waves[State.configWave] as SpawnWave;

            if (wave.spawners.Count == 0)
                return;

            // the spawner we are editing
            SpawnSpawner spawner = wave.spawners[State.configSpawner] as SpawnSpawner;

            for (int i = 0; i < spawner.spawnPointIdxs.Count; i++)
            {
                Handles.PositionHandle(sc.map.enemySpawnPoints[spawner.spawnPointIdxs[i]].position, Quaternion.identity);
            }

        }

        internal static class State{
            public static int configWave = 0;
            public static int configSpawner = 0;
            public static int configEnemy = 0;
        }
    }
}
