using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class TowerHealth : MonoBehaviour
{
    public int maxHp = 1000;
    public int currentHp;
    
    [Header("HPバー")]
    public Slider healthSlider;

    void Start()
    {
        currentHp = maxHp;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = currentHp;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        
        if (healthSlider != null)
        {
            healthSlider.value = currentHp;
        }

        if (currentHp <= 0)
        {
            if (gameObject.CompareTag("PlayerBase"))
            {
                SceneManager.LoadScene("GameOver");
            }
            else if (gameObject.CompareTag("EnemyBase"))
            {
                SceneManager.LoadScene("GameClear");
            }
            
            Destroy(gameObject);
        }
    }
}