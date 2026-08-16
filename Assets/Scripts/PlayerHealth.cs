using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public float invincibilityDuration = 1f;
    public float flickerInterval = 0.1f;
    private bool isInvincible = false;

    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    [Header("Health Regen")]
    [Tooltip("Aktif/nonaktifin auto-regen")]
    public bool regenEnabled = true;

    [Tooltip("Berapa detik player harus gak kena damage dulu sebelum regen mulai jalan")]
    public float regenDelay = 5f;

    [Tooltip("HP yang di-heal tiap satu 'tick' regen")]
    public int regenAmountPerTick = 5;

    [Tooltip("Jeda antar tick regen (detik)")]
    public float regenTickInterval = 1f;

    private float lastDamageTime = -Mathf.Infinity;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerMovement playerMovement;

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDeath;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();

        StartCoroutine(RegenWatcher());
    }

    public void TakeDamage(int damageAmount, Vector2 damageSourcePosition)
    {
        if (isInvincible || isDead) return;

        lastDamageTime = Time.time;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        Vector2 knockbackDirection = ((Vector2)transform.position - damageSourcePosition).normalized;
        StartCoroutine(ApplyKnockback(knockbackDirection));
        StartCoroutine(InvincibilityFlicker());
    }

    public void TakeDamage(int damageAmount)
    {
        TakeDamage(damageAmount, transform.position);
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        if (playerMovement != null) playerMovement.SetCanMove(false);

        rb.linearVelocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);

        if (playerMovement != null) playerMovement.SetCanMove(true);
    }

    private IEnumerator InvincibilityFlicker()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        isInvincible = false;
    }

    private IEnumerator RegenWatcher()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenTickInterval);

            if (!regenEnabled || isDead) continue;
            if (currentHealth >= maxHealth) continue;
            if (Time.time - lastDamageTime < regenDelay) continue;

            Heal(regenAmountPerTick);
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void InstantKill()
    {
        if (isDead) return;
        currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Die();
    }

    private void Die()
    {
        isDead = true;

        if (playerMovement != null)
        {
            playerMovement.StopAllCoroutines();
            playerMovement.enabled = false;
        }

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (animator != null) animator.SetTrigger("Death");
        OnPlayerDeath?.Invoke();
    }

    public int GetCurrentHealth() => currentHealth;
    public bool IsDead() => isDead;
}