using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuScreen;

    [Header("Settings")]
    [SerializeField] private GameObject SettingsPanel;

    [Header("Audio")]
    [SerializeField] private AudioClip menuMusic; // صدای پس‌زمینه منو
    private AudioSource audioSource;

    private void Awake()
    {
        mainMenuScreen.SetActive(true);
        SettingsPanel.SetActive(false);
        Time.timeScale = 1f;

        // آماده‌سازی پخش صدا
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f; // می‌تونی به دلخواه تغییر بدی
        audioSource.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingsPanel.activeInHierarchy)
            {
                CloseSettings();
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1); // بارگذاری صحنه با Index 1
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void OpenSettings()
    {
        mainMenuScreen.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
}
