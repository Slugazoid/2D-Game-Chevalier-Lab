using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Panel Pause yang mau ditampilkan (harus punya CanvasGroup)")]
    public CanvasGroup pausePanel;

    [Tooltip("Opsional — dipakai untuk cegah pause kalau player sudah mati (Game Over sedang tampil)")]
    public PlayerHealth playerHealth;

    [Tooltip("Drag PlayerMovement Player, biar kontrol player benar-benar mati total saat pause")]
    public PlayerMovement playerMovement;

    [Header("Fade")]
    public float fadeDuration = 0.2f;

    private bool isPaused = false;
    private bool isLocked = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.alpha = 0f;
            pausePanel.interactable = false;
            pausePanel.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLocked) return;
            if (playerHealth != null && playerHealth.IsDead()) return;

            TogglePause();
        }
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (isLocked && isPaused)
        {
            Resume();
        }
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (isPaused) return; // cegah double-call kalau ke-panggil 2x sebelum sempat toggle balik
        isPaused = true;

        Time.timeScale = 0f;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            if (playerMovement.Rigidbody2D != null)
            {
                playerMovement.Rigidbody2D.linearVelocity = Vector2.zero;
            }
        }

        if (pausePanel != null)
        {
            pausePanel.interactable = true;
            pausePanel.blocksRaycasts = true;
        }

        StartFade(1f);
    }

    // Attach ke OnClick() tombol "Resume" di Inspector
    public void Resume()
    {
        if (!isPaused) return; // cegah double-call
        isPaused = false;

        Time.timeScale = 1f;

        if (playerMovement != null) playerMovement.enabled = true;

        if (pausePanel != null)
        {
            pausePanel.interactable = false;
            pausePanel.blocksRaycasts = false;
        }

        StartFade(0f);

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (pausePanel == null) return;

        // Hentikan fade sebelumnya kalau masih jalan, biar gak tabrakan/ke-overlap
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(targetAlpha));
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        float startAlpha = pausePanel.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            pausePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        pausePanel.alpha = targetAlpha;
        fadeCoroutine = null;
    }

    // Attach ke OnClick() tombol "Restart" di Inspector
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Wajib reset waktu sebelum pindah scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Attach ke OnClick() tombol "Main Menu" di Inspector
    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Wajib reset waktu sebelum pindah scene

        // Cegah spam klik dan pastiin pause panel gak nyangkut aktif pas fade jalan
        if (pausePanel != null)
        {
            pausePanel.interactable = false;
            pausePanel.blocksRaycasts = false;
        }

        // Pindah ke Main Menu pake fade, sama kayak transisi Boss -> Credits
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene("MainMenu");
        }
        else
        {
            // Fallback kalau SceneFader belum ke-load di scene ini
            Debug.LogWarning("SceneFader.Instance null, load scene langsung tanpa fade.");
            SceneManager.LoadScene("MainMenu"); // Load nama scene Main Menu sesuai yang ada di Build Profiles
        }
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

    public bool IsPaused() => isPaused;
}