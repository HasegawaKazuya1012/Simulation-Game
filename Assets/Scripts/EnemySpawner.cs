using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("敵キャラの設定")]
    public GameObject enemyPrefab; 
    public Transform spawnPoint;   

    [Header("出撃タイミング")]
    public float spawnInterval = 5.0f; 
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; 
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}