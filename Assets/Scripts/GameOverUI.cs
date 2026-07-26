using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Drag GameObject Player yang punya komponen PlayerHealth")]
    public PlayerHealth playerHealth;

    [Tooltip("Panel Game Over yang mau ditampilkan (harus punya CanvasGroup)")]
    public CanvasGroup gameOverPanel;

    [Header("Timing")]
    [Tooltip("Jeda sebelum panel muncul, kasih waktu animasi death player muter dulu")]
    public float delayBeforeShow = 1.5f;
    public float fadeDuration = 0.5f;

    [Header("Opsional")]
    [Tooltip("Kalau true, game di-pause (Time.timeScale = 0) begitu panel selesai muncul")]
    public bool pauseGameOnShow = true;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= HandlePlayerDeath;
        }
    }

    private void Start()
    {
        // Pastikan panel tersembunyi total di awal game
        if (gameOverPanel != null)
        {
            gameOverPanel.alpha = 0f;
            gameOverPanel.interactable = false;
            gameOverPanel.blocksRaycasts = false;
        }
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(ShowGameOverSequence());
    }

    private IEnumerator ShowGameOverSequence()
    {
        // Tunggu dulu (pakai realtime karena nanti mungkin timeScale di-nol-kan)
        yield return new WaitForSecondsRealtime(delayBeforeShow);

        if (gameOverPanel == null) yield break;

        gameOverPanel.interactable = true;
        gameOverPanel.blocksRaycasts = true;

        // Fade in halus dari transparan ke terlihat penuh
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            gameOverPanel.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        gameOverPanel.alpha = 1f;

        if (pauseGameOnShow)
        {
            Time.timeScale = 0f;
        }
    }

    // Attach ke OnClick() tombol "Restart" di Inspector
    public void RestartLevel()
    {
        Time.timeScale = 1f; // wajib reset dulu sebelum reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Attach ke OnClick() tombol "Quit" di Inspector (opsional)
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}