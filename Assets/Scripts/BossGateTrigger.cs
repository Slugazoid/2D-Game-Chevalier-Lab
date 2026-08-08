using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossGateTrigger : MonoBehaviour
{
    [Header("Scene Target")]
    public string bossSceneName = "BossStage";

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.8f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null) pm.SetCanMove(false);

        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        if (fadeCanvasGroup != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        SceneManager.LoadScene(bossSceneName);
    }
}