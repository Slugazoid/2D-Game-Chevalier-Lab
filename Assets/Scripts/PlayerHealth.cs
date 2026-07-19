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

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerMovement playerMovement;

    public float deathDelay = 1.5f;
    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDeath;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(int damageAmount, Vector2 damageSourcePosition)
    {
        if (isInvincible || isDead) return;

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

    public void Heal(int healAmount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        if (animator != null) animator.SetTrigger("Death");

        if (playerMovement != null) playerMovement.enabled = false;

        OnPlayerDeath?.Invoke();
        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        yield return new WaitForSeconds(deathDelay);

        // TODO: ganti sesuai kebutuhan — reload scene, tampilkan Game Over screen, atau respawn di checkpoint
        // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public int GetCurrentHealth() => currentHealth;
    public bool IsDead() => isDead;
}