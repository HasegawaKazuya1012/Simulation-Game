using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("キャラクターのプレハブ")]
    public GameObject warriorPrefab;
    public GameObject lancerPrefab;
    public GameObject archerPrefab;
    public GameObject monkPrefab;

    [Header("出現場所")]
    public Transform spawnPoint;
    public void SpawnUnit(GameObject prefab)
    {
        UnitStatus status = prefab.GetComponent<UnitStatus>();

        if(status == null)
        {
            Debug.LogError("このキャラに UnitStatus スクリプトがない");
            return;
        }
        int cost = status.cost;

        if(GameManager.instance != null)
        {
            if (GameManager.instance.SpendMoney(cost))
            {
                Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                Debug.Log(prefab.name + "を出撃させました");
            }
            else
            {
                Debug.Log("お金が足りません");
            }
        }
        else
        {
            Debug.LogError("シーンに GameManager がありません！");
        }
    }
    public void OnClickWarrior() { SpawnUnit(warriorPrefab); }
    public void OnClickLancer()  { SpawnUnit(lancerPrefab); }
    public void OnClickArcher()  { SpawnUnit(archerPrefab); }
    public void OnClickMonk()    { SpawnUnit(monkPrefab); }
}