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
        SceneManager.LoadScene(1); // ایندکس صحنه آفلاین
    }

    public void StartOnlineMode()
    {
        SceneManager.LoadScene(0); // ایندکس صحنه آنلاین
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
