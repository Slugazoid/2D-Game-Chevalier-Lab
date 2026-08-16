using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Referensi")]
    public CanvasGroup pausePanel;

    public PlayerHealth playerHealth;

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
        if (isPaused) return;
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

    public void Resume()
    {
        if (!isPaused) return;
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

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.interactable = false;
            pausePanel.blocksRaycasts = false;
        }

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("SceneFader.Instance null, load scene langsung tanpa fade.");
            SceneManager.LoadScene("MainMenu");
        }
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