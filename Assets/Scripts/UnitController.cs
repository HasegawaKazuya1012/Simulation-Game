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

    void UpdateMonkAI()
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
            
            if (HasAllyAhead())
            {
                
                isStopped = false;
                SetMoveAnimation(true);
                Move();
            }
            else
            {
                
                isStopped = true;
                SetMoveAnimation(false);
            }
        }
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

    bool HasAllyAhead()
    {
        Vector2 direction = isPlayerTeam ? Vector2.right : Vector2.left;
        int targetLayerMask = 1 << LayerMask.NameToLayer(isPlayerTeam ? "PlayerTeam" : "EnemyTeam");

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 100f, targetLayerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                UnitController ally = hit.collider.GetComponent<UnitController>();
                if (ally != null)
                {
                    return true;
                }
            }
        }
        return false;
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