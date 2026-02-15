using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("設定")]
    public UnitStatus status; 
    public bool isPlayerTeam = true; 

    private Animator anim;
    private Rigidbody2D rb;
    private float currentHealth;
    private float attackCooldown = 0f;
    private bool isStopped = false; 

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        status = GetComponent<UnitStatus>(); 

        if (status != null)
        {
            currentHealth = status.maxHealth;
        }

        if (!isPlayerTeam)
        {
            // Scale（大きさ）のXを -1 にすると反転する
            Vector3 newScale = transform.localScale;
            newScale.x = -1;
            transform.localScale = newScale;
        }
    }

    void Update()
    {
        if (status == null) return;

        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

        GameObject target = DetectTarget();

        if (target != null)
        {
            // ターゲットがいる場合
            
            // 【Monkの特別処理】もし相手が味方で、HPが満タンなら無視して進む
            if (name.Contains("Monk"))
            {
                 UnitController friend = target.GetComponent<UnitController>();
                 if (friend != null && friend.IsFullHealth()) 
                 {
                     // 回復不要なので移動を続ける
                     Move();
                     SetMoveAnimation(true);
                     return; 
                 }
            }

            // 攻撃/回復のために止まる
            isStopped = true;
            SetMoveAnimation(false);
            
            if (attackCooldown <= 0)
            {
                PerformAction(target);
                attackCooldown = 2.0f; 
            }
        }
        else
        {
            // ターゲットがいない場合：移動
            isStopped = false;
            SetMoveAnimation(true);
            Move();
        }
    }

    void Move()
    {
        float direction = isPlayerTeam ? 1.0f : -1.0f;
        transform.Translate(Vector2.right * direction * status.moveSpeed * Time.deltaTime);
    }

    // 【重要修正】自分自身を検知しないように改良した索敵機能
    GameObject DetectTarget()
    {
        Vector2 direction = isPlayerTeam ? Vector2.right : Vector2.left;
        int targetLayerMask;
        
        if (name.Contains("Monk")) 
        {
             targetLayerMask = 1 << LayerMask.NameToLayer(isPlayerTeam ? "PlayerTeam" : "EnemyTeam");
        }
        else
        {
             targetLayerMask = 1 << LayerMask.NameToLayer(isPlayerTeam ? "EnemyTeam" : "PlayerTeam");
        }

        // ★修正点：ビームの発射位置を、自分の中心から少し（0.5）ずらす
        // これにより「自分自身のコライダー」に当たるのを防ぎます
        Vector2 startPos = (Vector2)transform.position + (direction * 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, status.attackRange, targetLayerMask);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void PerformAction(GameObject target)
    {
        anim.SetTrigger("Attack"); 
        
        if (name.Contains("Monk"))
        {
            UnitController ally = target.GetComponent<UnitController>();
            if (ally != null) ally.Heal(status.attackPower);
        }
        else
        {
            UnitController enemyUnit = target.GetComponent<UnitController>();
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(status.attackPower);
            }
            else
            {
                TowerHealth enemyTower = target.GetComponent<TowerHealth>();
                if (enemyTower != null) enemyTower.TakeDamage(status.attackPower);
            }
        }
    }

    void SetMoveAnimation(bool IsMoving)
    {
        if (anim != null) anim.SetBool("IsMoving", IsMoving);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > status.maxHealth) currentHealth = status.maxHealth;
    }

    // 【追加】HPが満タンか確認する機能
    public bool IsFullHealth()
    {
        return currentHealth >= status.maxHealth;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}