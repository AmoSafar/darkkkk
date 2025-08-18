using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelector : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuScreen;

    [Header("Audio")]
    [SerializeField] private AudioClip menuMusic; // صدای پس‌زمینه منو
    private AudioSource audioSource;

    private void Awake()
    {
        mainMenuScreen.SetActive(true);
        Time.timeScale = 1f;

        // آماده‌سازی پخش صدا
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    // --------------------- حالت‌ها ---------------------

    public void StartOfflineMode()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void StartOnlineMode()
    {
        SceneManager.LoadScene("SignUp&Login", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
