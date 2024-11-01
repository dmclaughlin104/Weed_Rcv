using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Variables
    public GameObject player;
    public GameObject enemyPrefab;
    public GameObject powerUpPrefab;
    private float spawnRange = 7;
    public int enemyCount;
    public int nextWave;
    private float playerSafetyZone = 5f;

    // Object pool variables
    public int initialPoolSize = 15; // Start with 30 enemies in the pool
    private List<GameObject> enemyPool; // Pool for enemies
    public List<GameObject> activeEnemies; // List to track currently active enemies

    // Game status
    public bool gameActive = false;

    void Start()
    {
        
        // Initialize the enemy pool
        enemyPool = new List<GameObject>();
        activeEnemies = new List<GameObject>();

        // Prepopulate the pool with inactive enemy objects
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false); // Initially set the enemy to inactive
            enemyPool.Add(enemy);
        }
    }

    void Update()
    {
        // Update the enemy count based on the number of active enemies
        enemyCount = activeEnemies.Count;

        // If the game is active, start gameplay spawning
        if (gameActive)
        {
            StartGameplay();
        }
    }

    // Method to start the main gameplay spawning loop
    public void StartGameplay()
    {
        // If all enemies are defeated, spawn more
        if (enemyCount == 0)
        {
            nextWave++; // Increase the number in the wave
            SpawnEnemyWave(nextWave);

            // Spawn a power-up every even-numbered wave (over 4)
            if (nextWave >= 4 && (nextWave % 2) == 0)
            {
                //SpawnPowerUp();
            }
        }
    }

    // Spawn an enemy wave
    void SpawnEnemyWave(int waveNumber)
    {
        // Spawn a number of enemies corresponding with the wave number
        for (int count = 0; count < waveNumber; count++)
        {
            SpawnEnemy();
            enemyCount++;
        }
    }

    // Method to spawn an enemy using object pooling
    void SpawnEnemy()
    {
        // Try to get an inactive enemy from the pool
        GameObject enemy = GetPooledEnemy();

        if (enemy != null)
        {
            // Set the enemy active and move it to the spawn position
            enemy.transform.position = GenerateSpawnPos(0);
            enemy.SetActive(true);
            activeEnemies.Add(enemy); // Add to the list of active enemies
        }
        else
        {
            // If no enemies are available, instantiate a new one and add it to the pool
            GameObject newEnemy = Instantiate(enemyPrefab, GenerateSpawnPos(0), enemyPrefab.transform.rotation);
            enemyPool.Add(newEnemy); // Add it to the pool
            activeEnemies.Add(newEnemy); // Add to active list
        }
    }

    // Method to get an inactive enemy from the pool
    GameObject GetPooledEnemy()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy; // Return the first inactive enemy found
            }
        }
        return null; // Return null if no inactive enemies are available
    }

    // Method to spawn a power-up
    public void SpawnPowerUp()
    {
        Instantiate(powerUpPrefab, GenerateSpawnPos(1.0f), powerUpPrefab.transform.rotation);
    }

    // Method to generate a new random spawn position
    public Vector3 GenerateSpawnPos(float objectYPosition)
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 spawnPos = new Vector3(spawnPosX, objectYPosition, spawnPosZ);

        // Ensure the spawn position isn't too close to the player position
        while (Vector3.Distance(spawnPos, player.transform.position) < playerSafetyZone)
        {
            spawnPosX = Random.Range(-spawnRange, spawnRange);
            spawnPosZ = Random.Range(-spawnRange, spawnRange);
            spawnPos = new Vector3(spawnPosX, objectYPosition, spawnPosZ);
        }

        return spawnPos;
    }

    // Method to deactivate an enemy and return it to the pool
    public void DeactivateEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemies.Remove(enemy);
    }

    public void DeactivateAllEnemies()
    {
        // Loop through all active enemies in the activeEnemies list
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            // Get the enemy from the list
            GameObject enemy = activeEnemies[i];

            // Get the EnemyController component attached to the enemy
            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            // Deactivate the enemy using the DeactivateEnemy method
            DeactivateEnemy(enemy);

            enemyController.ResetEnemyRB();
            enemyController.ResetEnemy();

        }
    }

    // Method to reset next wave back to zero - called in GameManager script
    public void ResetNextWave()
    {
        nextWave = 0;
    }
}
