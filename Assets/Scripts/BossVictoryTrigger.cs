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
        SceneManager.LoadScene(namaSceneCredits);
    }
}