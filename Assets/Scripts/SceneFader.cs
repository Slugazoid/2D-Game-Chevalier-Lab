using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton fade transition, mirip konsepnya sama SceneTransition autoload
// di project Godot lu. Taruh di scene paling awal (biasanya MainMenu),
// nempel terus lewat DontDestroyOnLoad, jadi bisa dipanggil dari scene manapun.
public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("Refs")]
    [Tooltip("CanvasGroup dari panel hitam full-screen yang nutupin layar")]
    public CanvasGroup fadeCanvasGroup;

    [Header("Timing")]
    public float fadeOutDuration = 0.6f;
    public float fadeInDuration = 0.6f;
    [Tooltip("Jeda pas layar udah item total, sebelum scene baru mulai fade in")]
    public float holdDuration = 0.1f;

    private void Awake()
    {
        // Pola singleton standar: kalau udah ada instance lain (misal balik lagi
        // ke scene yang isinya SceneFader juga), hancurin yang duplikat.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    // Panggil ini dari script manapun: SceneFader.Instance.FadeToScene("Credits");
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        yield return new WaitForSecondsRealtime(holdDuration);

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
    }

    // Time.unscaledDeltaTime dipake biar fade tetep jalan mulus walau
    // Time.timeScale lagi diubah (misal pause), sama kayak pola di MainMenuUI.cs
    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
        fadeCanvasGroup.blocksRaycasts = to > 0.01f;
    }
}