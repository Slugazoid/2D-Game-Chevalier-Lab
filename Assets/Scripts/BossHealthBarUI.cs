using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("Referensi")]
    public BossController bossController;

    public Slider healthSlider;
    public Image fillImage;
    public TextMeshProUGUI bossNameText;
    public string bossName = "BOSS";

    [Header("Format Angka")]
    public bool showHealthText = false;
    public TextMeshProUGUI healthText;

    [Header("Warna Fill")]
    public Color fillColor = new Color(0.9f, 0.15f, 0.15f); // merah

    [Header("Opsional: Smooth Animation")]
    public bool smoothTransition = true;
    public float smoothSpeed = 5f;

    private float targetValue;

    private void OnEnable()
    {
        if (bossController != null)
        {
            bossController.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (bossController != null)
        {
            bossController.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void Start()
    {
        if (bossController == null || healthSlider == null)
        {
            Debug.LogWarning("[BossHealthBarUI] Boss Controller atau Health Slider belum di-assign di Inspector.");
            return;
        }

        healthSlider.maxValue = bossController.maxHealth;
        healthSlider.value = bossController.maxHealth;
        targetValue = bossController.maxHealth;

        if (bossNameText != null) bossNameText.text = bossName;

        if (fillImage != null) fillImage.color = fillColor;
        UpdateHealthText(bossController.maxHealth, bossController.maxHealth);
    }

    private void Update()
    {
        if (smoothTransition && healthSlider.value != targetValue)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, smoothSpeed * Time.deltaTime);
            if (Mathf.Abs(healthSlider.value - targetValue) < 0.01f)
            {
                healthSlider.value = targetValue;
            }
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
        if (!showHealthText || healthText == null) return;
        healthText.text = $"{currentHealth}/{maxHealth}";
    }
}