using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("お金の設定")]
    public int currentMoney;
    public int maxMoney = 1000;
    public float increaseRate = 1.0f;

    [Header("UI設定")]
    public TextMeshProUGUI moneyText;
    [Header("Hpバー")]
    public Slider frontAllyHpBar;
    public Slider frontEnemyHpBar;

    public Vector3 EnemyhpBarOffSet = new Vector3(0,1.5f,0);
    public Vector3 AllyhpBarOffSet = new Vector3(0,2.0f,0);

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
        UpdateFrontHpUI();
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

    void UpdateFrontHpUI()
    {
        UnitController[] allUnit = FindObjectsOfType<UnitController>();
        UnitController frontAlly = null;
        UnitController frontEnemy = null;

        float maxAllyPos = -9999f;
        float maxEnemyPos = -9999f;

        foreach(UnitController unit in allUnit)
        {
            if(unit.isPlayerTeam)
            {
                if(unit.transform.position.x > maxAllyPos)
                {
                    maxAllyPos = unit.transform.position.x;
                    frontAlly = unit;
                }
            }
            else
            {
                if(-unit.transform.position.x > maxEnemyPos)
                {
                    maxEnemyPos = -unit.transform.position.x;
                    frontEnemy = unit;
                }
            }
        }
        if(frontAllyHpBar != null)
        {
            if(frontAlly != null)
            {
                frontAllyHpBar.gameObject.SetActive(true);
                frontAllyHpBar.maxValue = frontAlly.status.maxHealth;
                frontAllyHpBar.value = frontAlly.currentHealth;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(frontAlly.transform.position + AllyhpBarOffSet);
                frontAllyHpBar.transform.position = screenPos;
            }
            else
            {
                frontAllyHpBar.gameObject.SetActive(false);
            }
        }
        if(frontEnemyHpBar != null)
        {
            if (frontEnemy != null)
            {
                frontEnemyHpBar.gameObject.SetActive(true);
                frontEnemyHpBar.maxValue = frontEnemy.status.maxHealth;
                frontEnemyHpBar.value = frontEnemy.currentHealth;
                
                Vector3 screenPos = Camera.main.WorldToScreenPoint(frontEnemy.transform.position + EnemyhpBarOffSet);
                frontEnemyHpBar.transform.position = screenPos;
            }
            else
            {
                frontEnemyHpBar.gameObject.SetActive(false);
            }
        }
    }
}