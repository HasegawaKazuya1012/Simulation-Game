using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("敵キャラの設定")]
    public GameObject[] enemyPrefabs; 
    public Transform spawnPoint;   

    [Header("出撃タイミング")]
    public float spawnInterval = 5.0f; 
    public float minSpawnInterval = 2.0f; // 最小の出撃間隔
    public float speedUpRate = 0.05f; // 1秒ごとにどれくらい出撃間隔を短くするか
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if(spawnInterval > minSpawnInterval)
        {
            spawnInterval -= speedUpRate * Time.deltaTime;
        }

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; 
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length > 0 && spawnPoint != null)
        {
            int randomIndex = Random.Range(0,enemyPrefabs.Length);
            GameObject selectEnemy = enemyPrefabs[randomIndex];
            Instantiate(selectEnemy, spawnPoint.position, Quaternion.identity);
        }
    }
}