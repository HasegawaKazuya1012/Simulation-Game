using UnityEngine;

public class UnitStatus : MonoBehaviour
{
    [Header("ステータス設定")]
    public int maxHealth = 100;
    public int attackPower = 10;
    public float attackRange = 1.5f;
    public float moveSpeed = 2.0f;

    [Header("コスト設定")]
    public int cost = 100; // 生産にかかるお金
}