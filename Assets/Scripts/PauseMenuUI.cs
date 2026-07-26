using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float fadeTimer = 0f;
    private bool isFadingIn = false;
    private bool isFadingOut = false;

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

        HandleFade();
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

        isFadingIn = true;
        isFadingOut = false;
        fadeTimer = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (playerMovement != null) playerMovement.enabled = true;

        if (pausePanel != null)
        {
            pausePanel.interactable = false;
            pausePanel.blocksRaycasts = false;
        }

        isFadingOut = true;
        isFadingIn = false;
        fadeTimer = 0f;

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void HandleFade()
    {
        if (pausePanel == null) return;

        if (isFadingIn)
        {
            fadeTimer += Time.unscaledDeltaTime;
            pausePanel.alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            if (fadeTimer >= fadeDuration) isFadingIn = false;
        }
        else if (isFadingOut)
        {
            fadeTimer += Time.unscaledDeltaTime;
            pausePanel.alpha = 1f - Mathf.Clamp01(fadeTimer / fadeDuration);
            if (fadeTimer >= fadeDuration) isFadingOut = false;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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