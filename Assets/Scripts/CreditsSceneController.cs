using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CreditsSceneController : MonoBehaviour
{
    [Header("Refs")]
    public VideoPlayer videoPlayer;

    [Header("Pengaturan Scene")]
    public string namaSceneMainMenu = "MainMenu";

    [Header("Opsional")]
    [Tooltip("Boleh skip video pakai tombol ini")]
    public KeyCode skipKey = KeyCode.Escape;
    public bool allowSkip = true;

    private bool sudahPindahScene = false;

    private void OnEnable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += HandleVideoSelesai;
        }
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= HandleVideoSelesai;
        }
    }

    private void Update()
    {
        if (allowSkip && Input.GetKeyDown(skipKey))
        {
            GoToMainMenu();
        }
    }

    private void HandleVideoSelesai(VideoPlayer vp)
    {
        GoToMainMenu();
    }

    private void GoToMainMenu()
    {
        if (sudahPindahScene) return;
        sudahPindahScene = true;

        Time.timeScale = 1f; 
        SceneManager.LoadScene(namaSceneMainMenu);
    }
}