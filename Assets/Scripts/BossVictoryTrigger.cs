using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossVictoryTrigger : MonoBehaviour
{
    [Header("Refs")]
    public BossController bossController;

    [Header("Pengaturan Scene")]
    public string namaSceneCredits = "Credits";

    [Header("Timing")]
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
            Debug.LogWarning("SceneFader.Instance null, load scene langsung tanpa fade.");
            SceneManager.LoadScene(namaSceneCredits);
        }
    }
}