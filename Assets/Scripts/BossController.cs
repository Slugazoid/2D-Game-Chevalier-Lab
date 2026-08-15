using UnityEngine;
using System;
using System.Collections;

// Boss ground-melee dengan sistem fase. Fase cuma nge-tune angka yang udah
// ada (speed, cooldown, damage) berdasarkan sisa HP
public class BossController : MonoBehaviour, IDamageable
{
    private enum BossState { Idle, Chase, Attack, Hurt, Dead }
    private BossState currentState = BossState.Idle;

    [Header("Refs")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform player;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Movement (Phase 1 base value)")]
    public float moveSpeed = 2f;
    public float detectionRange = 8f;

    [Header("Attack (Phase 1 base value)")]
    public Transform attackPoint;
    public float attackRange = 1.2f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;
    [Tooltip("Jeda dari trigger animasi Attack sampe damage beneran kekasih (ganti manual, bukan lewat Animation Event lagi)")]
    public float attackDelay = 0.3f;
    private float nextAttackTime = 0f;

    [Header("Health & Phase")]
    public int maxHealth = 50;
    private int currentHealth;
    public float hurtDuration = 0.4f;
    private float hurtTimer = 0f;

    // Dengerin ini dari HealthBarUI buat update slider (sama pola kayak PlayerHealth.cs)
    public event Action<int, int> OnHealthChanged;

    [Tooltip("Pindah ke Fase 2 kalau sisa HP di bawah persentase ini")]
    [Range(0f, 1f)] public float phase2HealthPercent = 0.66f;
    [Tooltip("Pindah ke Fase 3 (enrage) kalau sisa HP di bawah persentase ini")]
    [Range(0f, 1f)] public float phase3HealthPercent = 0.33f;

    [Header("Phase 2 Multiplier")]
    public float phase2SpeedMult = 1.25f;
    public float phase2CooldownMult = 0.7f;   // makin kecil = makin sering nyerang
    public float phase2DamageMult = 1.15f;

    [Header("Phase 3 (Enrage) Multiplier")]
    public float phase3SpeedMult = 1.6f;
    public float phase3CooldownMult = 0.45f;
    public float phase3DamageMult = 1.4f;

    private int currentPhase = 1;

    // nilai efektif yang beneran dipakai di gameplay, dihitung ulang tiap ganti fase
    private float effectiveMoveSpeed;
    private float effectiveCooldown;
    private int effectiveDamage;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
        RecalculatePhaseStats();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Langsung ngadep ke player begitu fight mulai, apapun posisi awal boss.
        if (player != null)
        {
            float initialDirection = player.position.x > transform.position.x ? 1f : -1f;
            FlipSprite(initialDirection);
        }
    }

    void Update()
    {
        if (currentState == BossState.Dead) return;

        isGrounded = groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (currentState == BossState.Hurt)
        {
            hurtTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (hurtTimer <= 0f) currentState = BossState.Idle;

            if (animator != null) animator.SetFloat("Speed", 0f);
            return;
        }

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = BossState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = BossState.Chase;
        }
        else
        {
            currentState = BossState.Idle;
        }

        switch (currentState)
        {
            case BossState.Idle:
                Idle();
                break;
            case BossState.Chase:
                Chase();
                break;
            case BossState.Attack:
                Attack();
                break;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("isJumping", !isGrounded);
        }
    }

    private void Idle()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void Chase()
    {
        float direction = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * effectiveMoveSpeed, rb.linearVelocity.y);
        FlipSprite(direction);
    }

    private void Attack()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        float direction = player.position.x > transform.position.x ? 1f : -1f;
        FlipSprite(direction);

        if (Time.time >= nextAttackTime)
        {
            if (animator != null)
            {
                animator.SetInteger("AttackIndex", UnityEngine.Random.Range(0, 2)); // 0 = BossAttack1, 1 = BossAttack2
                animator.SetTrigger("Attack");
            }
            StartCoroutine(DealDamageAfterDelay());
            nextAttackTime = Time.time + effectiveCooldown;
        }
    }

    // Ganti Animation Event: damage kekasih otomatis attackDelay detik setelah trigger,
    // ga perlu nempelin event manual di tiap clip attack lagi.
    private IEnumerator DealDamageAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);

        if (currentState == BossState.Dead) yield break;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange);
        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(effectiveDamage, transform.position);
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        currentState = BossState.Hurt;
        hurtTimer = hurtDuration;
        if (animator != null) animator.SetTrigger("Hurt");

        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        float healthPercent = (float)currentHealth / maxHealth;
        int newPhase = currentPhase;

        if (healthPercent <= phase3HealthPercent) newPhase = 3;
        else if (healthPercent <= phase2HealthPercent) newPhase = 2;
        else newPhase = 1;

        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            RecalculatePhaseStats();
            StartCoroutine(PhaseTransitionFlash());
        }
    }

    private void RecalculatePhaseStats()
    {
        switch (currentPhase)
        {
            case 3:
                effectiveMoveSpeed = moveSpeed * phase3SpeedMult;
                effectiveCooldown = attackCooldown * phase3CooldownMult;
                effectiveDamage = Mathf.RoundToInt(attackDamage * phase3DamageMult);
                break;
            case 2:
                effectiveMoveSpeed = moveSpeed * phase2SpeedMult;
                effectiveCooldown = attackCooldown * phase2CooldownMult;
                effectiveDamage = Mathf.RoundToInt(attackDamage * phase2DamageMult);
                break;
            default:
                effectiveMoveSpeed = moveSpeed;
                effectiveCooldown = attackCooldown;
                effectiveDamage = attackDamage;
                break;
        }
    }

    // Feedback visual sederhana pas ganti fase - kedip putih/oranye sebentar.
    private IEnumerator PhaseTransitionFlash()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        Color flashColor = currentPhase == 3 ? new Color(1f, 0.3f, 0.2f) : new Color(1f, 0.8f, 0.3f);

        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.08f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.08f);
        }
    }

    private void Die()
    {
        currentState = BossState.Dead;
        rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null) animator.SetTrigger("Death");

        // TODO: panggil UI kemenangan / balik ke MainMenu di sini kalau udah ada.
    }

    public int GetCurrentHealth() => currentHealth;
    public bool IsDead() => currentState == BossState.Dead;

    private void FlipSprite(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = direction > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}