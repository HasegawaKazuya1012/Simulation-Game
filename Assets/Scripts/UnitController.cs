using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("設定")]
    public UnitStatus status; 
    public bool isPlayerTeam = true; 

    private Animator anim;
    private Rigidbody2D rb;
    public float currentHealth;
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
            Vector3 newScale = transform.localScale;
            newScale.x = -1;
            transform.localScale = newScale;
        }
    }

    void Update()
    {
        if (status == null) return;

        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
        
        if (name.Contains("Monk"))
        {
            UpdateMonkAI(); 
        }
        else
        {
            UpdateNormalAI(); 
        }
    }

    // ==============================================
    // Monk（僧侶）専用のAI
    // ==============================================
    void UpdateMonkAI()
    {
        GameObject frontAlly = GetFrontAlly();

        // 1. Monk以外の味方がいないなら待機
        if (frontAlly == null)
        {
            isStopped = true;
            SetMoveAnimation(false);
            return;
        }

        float myForwardPos = isPlayerTeam ? transform.position.x : -transform.position.x;
        float targetForwardPos = isPlayerTeam ? frontAlly.transform.position.x : -frontAlly.transform.position.x;
        float distance = Mathf.Abs(transform.position.x - frontAlly.transform.position.x);

        // 4. 最も前にいる味方がMonkより後方にいるなら待機する
        if (targetForwardPos <= myForwardPos)
        {
            isStopped = true;
            SetMoveAnimation(false);
            return;
        }

        // --- ここから下は「最前線の味方が自分より前方にいる」ことが確定 ---

        if (distance <= status.attackRange)
        {
            // 2. Monkより前方にいて、射程距離より近くにいるなら止まって回復させる
            isStopped = true;
            SetMoveAnimation(false);

            if (attackCooldown <= 0)
            {
                PerformAction(frontAlly); // 必ず最前線のキャラを回復する
                attackCooldown = 2.0f;
            }
        }
        else
        {
            // 3. 射程距離より遠くにいるなら射程距離まで移動する
            // 【重要】Archerなどで立ち往生しないよう、味方との重なり判定を無くし、すり抜けて前進させます
            isStopped = false;
            SetMoveAnimation(true);
            Move();
        }
    }

    // Monk自身と他のMonkを除く、最も前にいる味方を取得する
    GameObject GetFrontAlly()
    {
        UnitController[] allUnits = FindObjectsOfType<UnitController>();
        GameObject frontUnit = null;
        float maxForwardPos = -9999f;

        foreach (UnitController unit in allUnits)
        {
            // 同じチーム、自分ではない、そして「Monkではない」味方だけを探す
            if (unit.isPlayerTeam == this.isPlayerTeam && unit.gameObject != this.gameObject && !unit.name.Contains("Monk"))
            {
                float forwardPos = isPlayerTeam ? unit.transform.position.x : -unit.transform.position.x;

                if (forwardPos > maxForwardPos)
                {
                    maxForwardPos = forwardPos;
                    frontUnit = unit.gameObject;
                }
            }
        }
        return frontUnit;
    }

    void UpdateNormalAI()
    {
        GameObject target = DetectTarget();

        if (target != null)
        {
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
            isStopped = false;
            SetMoveAnimation(true);
            Move();
        }
    }

    void Move()
    {
        if (isStopped) return;
        float direction = isPlayerTeam ? 1.0f : -1.0f;
        transform.Translate(Vector2.right * direction * status.moveSpeed * Time.deltaTime);
    }

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

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, status.attackRange, targetLayerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                UnitController unit = hit.collider.GetComponent<UnitController>();
                if (unit != null)
                {
                    return hit.collider.gameObject; 
                }
                
                if (!name.Contains("Monk"))
                {
                    TowerHealth tower = hit.collider.GetComponent<TowerHealth>();
                    if (tower != null)
                    {
                        return hit.collider.gameObject; 
                    }
                }
            }
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

    public bool IsFullHealth()
    {
        return currentHealth >= status.maxHealth;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if (status != null)
        {
            Gizmos.color = Color.red;
            Vector2 direction = isPlayerTeam ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(transform.position, direction * status.attackRange);
        }
    }
}