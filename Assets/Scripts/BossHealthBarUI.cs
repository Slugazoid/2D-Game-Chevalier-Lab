using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Pola sama kayak HealtBarUI.cs (punya player), tapi dengerin BossController
// dan nambahin ganti warna fill sesuai fase HP boss.
public class BossHealthBarUI : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Drag GameObject Boss yang punya komponen BossController")]
    public BossController bossController;

    [Tooltip("Drag komponen Slider yang jadi health bar")]
    public Slider healthSlider;

    [Tooltip("Drag Image yang jadi fill si Slider (buat ganti warna per fase). Boleh dikosongkan kalau ga perlu ganti warna.")]
    public Image fillImage;

    [Tooltip("Drag komponen TextMeshProUGUI untuk nampilin nama boss (opsional)")]
    public TextMeshProUGUI bossNameText;
    public string bossName = "BOSS";

    [Header("Format Angka")]
    [Tooltip("True = tampil '35/50', False = cuma bar tanpa angka")]
    public bool showHealthText = false;
    public TextMeshProUGUI healthText;

    [Header("Warna per Fase")]
    public Color phase1Color = new Color(0.2f, 0.85f, 0.3f); // hijau
    public Color phase2Color = new Color(1f, 0.8f, 0.1f);    // kuning
    public Color phase3Color = new Color(0.9f, 0.15f, 0.15f); // merah (enrage)

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

        UpdateFillColor(1f);
        UpdateHealthText(bossController.maxHealth, bossController.maxHealth);
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

        float percent = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        UpdateFillColor(percent);
        UpdateHealthText(currentHealth, maxHealth);
    }

    // Warnanya nyontek threshold yang sama kayak phase2HealthPercent/phase3HealthPercent
    // di BossController, biar bar berubah warna pas boss ganti fase beneran.
    private void UpdateFillColor(float healthPercent)
    {
        if (fillImage == null || bossController == null) return;

        if (healthPercent <= bossController.phase3HealthPercent)
            fillImage.color = phase3Color;
        else if (healthPercent <= bossController.phase2HealthPercent)
            fillImage.color = phase2Color;
        else
            fillImage.color = phase1Color;
    }

    private void UpdateHealthText(int currentHealth, int maxHealth)
    {
        if (!showHealthText || healthText == null) return;
        healthText.text = $"{currentHealth}/{maxHealth}";
    }
}