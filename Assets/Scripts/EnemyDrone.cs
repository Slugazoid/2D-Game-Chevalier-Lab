using UnityEngine;
using System.Collections;

public class EnemyDrone : MonoBehaviour, IDamageable
{
    private enum EnemyState { Patrol, Chase, Attack, Hurt, Dead }

    public Rigidbody2D rb;
    public Animator animator;
    public Transform player;

    public float patrolDistance = 3f;
    public float patrolSpeed = 1.5f;
    public float hoverHeight = 0.3f;
    public float hoverSpeed = 2f;

    public float detectionRange = 6f;
    public float attackRange = 4f;
    public float chaseSpeed = 3f;

    public float attackCooldown = 1.5f;
    public int attackDamage = 1;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float nextAttackTime = 0f;

    public int maxHealth = 3;
    private int currentHealth;
    private float hurtDuration = 0.4f;
    private float hurtTimer = 0f;

    [Header("Turning")]
    public float turnAnimationDuration = 0.5f;
    private bool isTurning = false;

    private EnemyState currentState = EnemyState.Patrol;
    private Vector3 startPosition;
    private int patrolDirection = -1;

    void Start()
    {
        startPosition = transform.position;
        currentHealth = maxHealth;

        if (rb != null)
        {
            rb.gravityScale = 0f;
        }

        FlipSprite(patrolDirection);

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (currentState == EnemyState.Dead) return;

        if (currentState == EnemyState.Hurt)
        {
            hurtTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            if (hurtTimer <= 0f) currentState = EnemyState.Patrol;

            if (animator != null) animator.SetFloat("Speed", 0f);
            return;
        }

        if (isTurning)
        {
            if (animator != null) animator.SetFloat("Speed", 0f);
            return;
        }

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    private void Patrol()
    {
        float distanceFromStart = transform.position.x - startPosition.x;
        int newDirection = patrolDirection;

        if (distanceFromStart >= patrolDistance) newDirection = -1;
        else if (distanceFromStart <= -patrolDistance) newDirection = 1;

        if (newDirection != patrolDirection)
        {
            StartCoroutine(TurnThenPatrol(newDirection));
            return;
        }

        ApplyHoverMovement(patrolDirection);
    }

    private IEnumerator TurnThenPatrol(int newDirection)
    {
        isTurning = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (animator != null) animator.SetTrigger("Turn");

        yield return new WaitForSeconds(turnAnimationDuration);

        patrolDirection = newDirection;
        FlipSprite(patrolDirection);
        isTurning = false;
    }

    private void ApplyHoverMovement(int direction)
    {
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        float targetY = startPosition.y + hoverOffset;
        float verticalVelocity = (targetY - transform.position.y) * hoverSpeed;

        rb.linearVelocity = new Vector2(direction * patrolSpeed, verticalVelocity);
    }

    private void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        FlipSprite(player.position.x > transform.position.x ? 1 : -1);
    }

    private void Attack()
    {
        rb.linearVelocity = Vector2.zero;
        if (Time.time >= nextAttackTime)
        {
            if (animator != null) animator.SetTrigger("Attack");

            if (projectilePrefab != null && firePoint != null)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Vector2 dirToPlayer = (player.position - firePoint.position).normalized;
                Projectile projectileScript = proj.GetComponent<Projectile>();
                projectileScript.SetDirection(dirToPlayer);
                projectileScript.damage = attackDamage;
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentState == EnemyState.Dead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            currentState = EnemyState.Hurt;
            hurtTimer = hurtDuration;
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(hurtDuration);
        sr.color = originalColor;
    }

    private void Die()
    {
        currentState = EnemyState.Dead;
        rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null) animator.SetTrigger("Death");

        Destroy(gameObject, 1.5f);
    }

    private void FlipSprite(int direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = direction > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}