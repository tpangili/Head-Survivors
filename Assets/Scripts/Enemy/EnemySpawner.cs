using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // Groups of enemy types in this wave
        public int waveQuota;       // Total number of enemies to spawn in this wave
        public float spawnInterval; // Interval at which to spawn enemies
        public float spawnCount;    // Number of enemies already spawned in this wave
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;      // Number of this enemy type to spawn in this wave
        public int spawnCount;      // Number of this enemy type already spawned
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; // A list of all the waves 
    public int currentWaveCount; // Index of current wave (starts at 0)

    [Header("Spawn Attributes")]
    float spawnTimer; // Determines when to spawn next enemy
    public int enemiesAlive;
    public int maxEnemiesAllowed; // Maximum number of enemies allowed on the map at once
    public int maxEnemyIncrease; // How much to increase max enemies after each wave
    public bool maxEnemiesReached = false;
    public float waveInterval; // Interval between each wave
    bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; // List of all relative spawn points for enemies

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if wave has ended and next wave should start
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }
        spawnTimer += Time.deltaTime;

        // Checks if it's time to spawn more enemies
        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        // Wave happens for "waveInterval" seconds before starting next wave
        yield return new WaitForSeconds(waveInterval);
        //Debug.Log("wave interval done");

        // If there are more waves after the current wave, start the next one
        if(currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            maxEnemiesAllowed += maxEnemyIncrease; // increases the max enemies allowed
            //Debug.Log(currentWaveCount);
            CalculateWaveQuota();
        }
    }

    // Calculates based on how many of each enemy type needs to be spawned
    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        // Checks how much of each enemy type needs to be spawned
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        //Debug.LogWarning(currentWaveQuota);
    }

    // This function will stop spawning enemies if the amount of enemies is at maximum
    // Will only spawn enemies in a particular wave until it is time for the next wave
    void SpawnEnemies()
    {
        // Checks if the minimum number of enemies in the wave have been spawned
        if(waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            // Spawn each type of enemy until quota is filled
            foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Check if the minimum number of enemies of this type have been spawned
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[UnityEngine.Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);
                    
                    /*Vector2 spawnPosition = new Vector2(player.transform.position.x + UnityEngine.Random.Range(-10f, 10f), player.transform.position.y + UnityEngine.Random.Range(-10f, 10f));
                    Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);*/

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    // Limits number of enemies that can be spawned at once
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    // Call when enemy is killed
    public void OnEnemyKilled()
    {
        enemiesAlive--;
        // Checks if number of enemies has fallen below maximum
        if(enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
