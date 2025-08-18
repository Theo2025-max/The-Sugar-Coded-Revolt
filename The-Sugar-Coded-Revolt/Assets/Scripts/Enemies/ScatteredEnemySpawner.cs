using UnityEngine;

public class ScatteredEnemySpawner : MonoBehaviour
{
    [Header("Main Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    public float respawnRadius = 20f;
    public float minDistanceFromPlayer = 5f;
    public float respawnDelay = 5f;

    [Header("Spawn Protection")]
    public float minDistanceBetweenEnemies = 3f;

    private Transform player;
    private int currentEnemyCount = 0;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnInitialEnemies();
        InvokeRepeating("RespawnEnemies", respawnDelay, respawnDelay);
    }

    void SpawnInitialEnemies()
    {
        while (currentEnemyCount < maxEnemies)
        {
            SpawnSingleEnemy();
        }
    }

    void RespawnEnemies()
    {
        currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemyCount < maxEnemies)
        {
            SpawnSingleEnemy();
        }
    }

    void SpawnSingleEnemy()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemyCount++;
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 30; i++) // Max 30 attempts
        {
            Vector3 randomPoint = Random.insideUnitSphere * respawnRadius;
            randomPoint.y = 0; // Keep on same level
            Vector3 spawnPos = player.position + randomPoint;

            // Check distance from player
            if (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer)
                continue;

            // Check distance from other enemies
            bool positionValid = true;
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (Vector3.Distance(spawnPos, enemy.transform.position) < minDistanceBetweenEnemies)
                {
                    positionValid = false;
                    break;
                }
            }

            if (positionValid)
                return spawnPos;
        }

        return Vector3.zero; // Failed to find position
    }
}
