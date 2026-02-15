using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("お金の設定")]
    public int currentMoney;
    public int maxMoney = 1000;
    public float increaseRate = 1.0f;

    [Header("UI設定")]
    public TextMeshProUGUI moneyText;

    private float timer;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentMoney = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= increaseRate)
        {
            AddMoney(10);
            timer = 0;
        }
    }

    void AddMoney(int amount)
    {
        currentMoney += amount;
        if (currentMoney >= maxMoney) currentMoney = maxMoney;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + currentMoney + "/" + maxMoney;
        }
    }
}