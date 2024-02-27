using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public List<Transform> spawnPoints = new List<Transform>();
    private Transform lastSpawnPointUsed;

    public LogManager logManager;

    private int killCounter;

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        // E�er spawn noktalar� listesi bo�sa spawn i�lemi yapma
        if (spawnPoints.Count == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Spawn points or enemy prefabs are not set!");
            return;
        }

        // Rastgele bir d��man prefab� se�
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // E�er son spawn noktas� yoksa veya son spawn noktas� kullan�lmad�ysa, rastgele bir spawn noktas� se�
        if (lastSpawnPointUsed == null || !spawnPoints.Contains(lastSpawnPointUsed))
        {
            lastSpawnPointUsed = spawnPoints[Random.Range(0, spawnPoints.Count)];
        }
        else
        {
            // Son spawn noktas� d���nda rastgele bir spawn noktas� se�
            List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
            availableSpawnPoints.Remove(lastSpawnPointUsed);
            lastSpawnPointUsed = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        }

        // Z konumunu 0 olarak ayarla
        Vector3 spawnPosition = new Vector3(lastSpawnPointUsed.position.x, lastSpawnPointUsed.position.y, 0f);

        // Se�ilen spawn noktas�nda rastgele bir d��man prefab� olu�tur
        GameObject newEnemy = Instantiate(randomEnemyPrefab, spawnPosition, Quaternion.identity);
    }

    public void EnemyKilled()
    {
        // Bir d��man �ld�r�ld���nde, bir sonraki d��man� do�ur
        SpawnEnemy();
        killCounter++;
        logManager.Log("Kill: " + killCounter);
    }
}
