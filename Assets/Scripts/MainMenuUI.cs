using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panel")]
    [Tooltip("Panel Main Menu utama (harus punya CanvasGroup)")]
    public CanvasGroup mainMenuPanel;

    [Tooltip("Panel Settings (harus punya CanvasGroup)")]
    public CanvasGroup settingsPanel;

    [Header("Pengaturan Scene")]
    [Tooltip("Nama Scene Gameplay yang mau di-load (Pastikan sama persis dengan di Build Profiles)")]
    public string namaSceneGameplay = "MainGame";

    [Header("Fade")]
    public float fadeDuration = 0.3f;

    private Coroutine fadeMenuCoroutine;
    private Coroutine fadeSettingsCoroutine;

    private void Start()
    {
        Time.timeScale = 1f;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.alpha = 1f;
            mainMenuPanel.interactable = true;
            mainMenuPanel.blocksRaycasts = true;
        }

        if (settingsPanel != null)
        {
            settingsPanel.alpha = 0f;
            settingsPanel.interactable = false;
            settingsPanel.blocksRaycasts = false;
        }
    }

    // Attach ke OnClick() tombol "Play"
    public void PlayGame()
    {
        // Cegah player spam klik tombol Play dua kali
        if (mainMenuPanel != null)
        {
            mainMenuPanel.interactable = false;
        }

        // Pindah ke Scene gameplay pake fade, sama kayak transisi Boss -> Credits
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(namaSceneGameplay);
        }
        else
        {
            // Fallback kalau SceneFader belum ke-load di scene ini (harusnya jarang
            // kejadian karena MainMenu biasanya scene pertama yang dibuka).
            Debug.LogWarning("SceneFader.Instance null, load scene langsung tanpa fade.");
            SceneManager.LoadScene(namaSceneGameplay);
        }
    }

    // Attach ke OnClick() tombol "Settings"
    public void OpenSettings()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.interactable = false;
            mainMenuPanel.blocksRaycasts = false;
        }
        StartFade(mainMenuPanel, ref fadeMenuCoroutine, 0f);

        if (settingsPanel != null)
        {
            settingsPanel.interactable = true;
            settingsPanel.blocksRaycasts = true;
        }
        StartFade(settingsPanel, ref fadeSettingsCoroutine, 1f);
    }

    // Attach ke OnClick() tombol "Back" di panel Settings
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.interactable = false;
            settingsPanel.blocksRaycasts = false;
        }
        StartFade(settingsPanel, ref fadeSettingsCoroutine, 0f);

        if (mainMenuPanel != null)
        {
            mainMenuPanel.interactable = true;
            mainMenuPanel.blocksRaycasts = true;
        }
        StartFade(mainMenuPanel, ref fadeMenuCoroutine, 1f);
    }

    // Attach ke OnClick() tombol "Quit"
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void StartFade(CanvasGroup target, ref Coroutine coroutineRef, float targetAlpha)
    {
        if (target == null) return;
        if (coroutineRef != null) StopCoroutine(coroutineRef);
        coroutineRef = StartCoroutine(FadeCanvasGroup(target, targetAlpha));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha)
    {
        float startAlpha = group.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }
        group.alpha = targetAlpha;
    }
}