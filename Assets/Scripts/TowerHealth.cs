using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ★重要：シーン切り替えに必要！

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
            // HPが0になったら、タグを見てどっちの城か判断する
            if (gameObject.CompareTag("PlayerBase"))
            {
                // 自分の城（PlayerBase）が壊れた → 負け
                SceneManager.LoadScene("GameOver");
            }
            else if (gameObject.CompareTag("EnemyBase"))
            {
                // 敵の城（EnemyBase）が壊れた → 勝ち
                SceneManager.LoadScene("GameClear");
            }
            
            // 城を消す
            Destroy(gameObject);
        }
    }
}