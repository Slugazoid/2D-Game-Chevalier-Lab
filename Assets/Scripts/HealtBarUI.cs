using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Drag GameObject Player yang punya komponen PlayerHealth")]
    public PlayerHealth playerHealth;

    [Tooltip("Drag komponen Slider yang jadi health bar")]
    public Slider healthSlider;

    [Tooltip("Drag komponen TextMeshProUGUI untuk nampilin angka HP (opsional, boleh dikosongkan)")]
    public TextMeshProUGUI healthText;

    [Header("Format Angka")]
    [Tooltip("True = tampil '75/100', False = tampil '75' saja")]
    public bool showMaxHealth = true;

    [Header("Opsional: Smooth Animation")]
    [Tooltip("Kalau true, bar bergerak halus (tidak langsung loncat) saat HP berubah")]
    public bool smoothTransition = true;
    public float smoothSpeed = 5f;

    private float targetValue;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void Start()
    {
        if (playerHealth == null || healthSlider == null)
        {
            Debug.LogWarning("[HealthBarUI] Player Health atau Health Slider belum di-assign di Inspector.");
            return;
        }

        healthSlider.maxValue = playerHealth.maxHealth;
        healthSlider.value = playerHealth.maxHealth;
        targetValue = playerHealth.maxHealth;

        UpdateHealthText(playerHealth.maxHealth, playerHealth.maxHealth);
    }

    private void Update()
    {
        if (smoothTransition && healthSlider.value != targetValue)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, smoothSpeed * Time.deltaTime);
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;

        if (smoothTransition)
        {
            targetValue = currentHealth;
        }
        else
        {
            healthSlider.value = currentHealth;
        }

        UpdateHealthText(currentHealth, maxHealth);
    }

    private void UpdateHealthText(int currentHealth, int maxHealth)
    {
        if (healthText == null) return;

        healthText.text = showMaxHealth
            ? $"{currentHealth}/{maxHealth}"
            : $"{currentHealth}";
    }
}