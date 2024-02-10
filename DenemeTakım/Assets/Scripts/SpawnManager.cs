using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
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
        // Eðer son spawn noktasý yoksa veya spawn noktalarý listesi boþsa, rastgele bir spawn noktasý seç
        if (lastSpawnPointUsed == null || spawnPoints.Count == 0)
        {
            lastSpawnPointUsed = spawnPoints[Random.Range(0, spawnPoints.Count)];
        }
        else
        {
            // Son spawn noktasý dýþýnda rastgele bir spawn noktasý seç
            List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
            availableSpawnPoints.Remove(lastSpawnPointUsed);
            lastSpawnPointUsed = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        }

        // Z konumunu 0 olarak ayarla
        Vector3 spawnPosition = new Vector3(lastSpawnPointUsed.position.x, lastSpawnPointUsed.position.y, 0f);

        // Seçilen spawn noktasýnda düþmaný oluþtur
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
    public void EnemyKilled()
    {
        // Bir düþman öldürüldüðünde, bir sonraki düþmaný doður
        SpawnEnemy();
        killCounter++;
        logManager.Log("Kill: " + killCounter);
    }
}
