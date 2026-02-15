using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("敵キャラの設定")]
    public GameObject enemyPrefab; // 出撃させる敵のプレハブ
    public Transform spawnPoint;   // 出現位置

    [Header("出撃タイミング")]
    public float spawnInterval = 5.0f; // 何秒ごとに敵を出すか
    private float timer = 0f;

    void Update()
    {
        // 時間を数える
        timer += Time.deltaTime;

        // 時間が来たら出撃
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; // タイマーをリセット
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            // 敵を生み出す
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}