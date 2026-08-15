using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Nempel di scene BossStage. Dengerin BossController.OnBossDeath,
// tunggu animasi Death boss selesai, terus pindah ke scene Credits.
public class BossVictoryTrigger : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Drag GameObject Boss yang punya komponen BossController")]
    public BossController bossController;

    [Header("Pengaturan Scene")]
    public string namaSceneCredits = "Credits";

    [Header("Timing")]
    [Tooltip("Jeda sebelum pindah scene, kasih waktu animasi Death boss muter dulu")]
    public float delayBeforeTransition = 2f;

    private void OnEnable()
    {
        if (bossController != null)
        {
            bossController.OnBossDeath += HandleBossDeath;
        }
    }

    private void OnDisable()
    {
        if (bossController != null)
        {
            bossController.OnBossDeath -= HandleBossDeath;
        }
    }

    private void HandleBossDeath()
    {
        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        yield return new WaitForSecondsRealtime(delayBeforeTransition);

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(namaSceneCredits);
        }
        else
        {
            // Fallback kalau SceneFader belum ke-load (misal Play Mode langsung
            // dari scene BossStage tanpa lewat MainMenu dulu pas testing).
            Debug.LogWarning("SceneFader.Instance null, load scene langsung tanpa fade.");
            SceneManager.LoadScene(namaSceneCredits);
        }
    }
}