using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIGameOverManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Health player1Health;
    [SerializeField] private Health player2Health;

    [Header("Game Over")]
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private AudioClip GameOverSound;

    [Header("Win")]
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private AudioClip WinSound;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Pause")]
    [SerializeField] private GameObject PauseScreen;

    [Header("Settings")]
    [SerializeField] private GameObject SettingsPanel;

    [Header("Restart Settings")]
    [SerializeField] private int restartSceneIndex = 1;

    private bool gameEnded = false;

    private AudioSource audioSource;

    private void Awake()
    {
        GameOverScreen.SetActive(false);
        WinScreen.SetActive(false);
        SettingsPanel.SetActive(false);

        // اضافه کردن یا گرفتن AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;

        // شروع پخش موزیک پس زمینه
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    private void Update()
    {
        // اگر هر دو پلیر مردند گیم اور شود
        if (!gameEnded && player1Health.currentHealth <= 0 && player2Health.currentHealth <= 0)
        {
            gameEnded = true;
            GameOver();
        }

        // اینجا می‌تونی اضافه کنی که وقتی برنده شدی موسیقی پس‌زمینه متوقف بشه یا عوض بشه

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingsPanel.activeInHierarchy)
            {
                CloseSettings();
            }
            else
            {
                if (PauseScreen.activeInHierarchy)
                    PauseGame(false);
                else
                    PauseGame(true);
            }
        }
    }

    public void Resume()
    {
        PauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        GameOverScreen.SetActive(true);

        // قطع موسیقی پس زمینه در صورت تمایل
        if (audioSource.isPlaying)
            audioSource.Stop();

        if (GameOverSound != null && SoundManager.Instance != null)
        {
            PlayClipIgnorePause(GameOverSound, SoundManager.Instance.SFXVolume);
        }
        else
        {
            if (GameOverSound == null)
                Debug.LogWarning("GameOverSound is not assigned in UIGameOverManager!");
            if (SoundManager.Instance == null)
                Debug.LogWarning("SoundManager instance not found!");
        }
    }

    public void Win()
    {
        if (WinScreen != null)
            WinScreen.SetActive(true);
        else
            Debug.LogError("WinScreen is not assigned in UIGameOverManager!");

        // قطع موسیقی پس زمینه در صورت تمایل
        if (audioSource.isPlaying)
            audioSource.Stop();

        if (WinSound != null)
        {
            Vector3 pos = (Camera.main != null) ? Camera.main.transform.position : Vector3.zero;
            float volume = 1f;
            if (SoundManager.Instance != null)
                volume = SoundManager.Instance.SFXVolume;

            AudioSource.PlayClipAtPoint(WinSound, pos, volume);
        }
        else
        {
            Debug.LogWarning("WinSound clip not assigned!");
        }
    }

    private void PlayClipIgnorePause(AudioClip clip, float volume)
    {
        if (clip == null) return;

        if (FindObjectsOfType<AudioListener>().Length == 0)
        {
            var listenerGO = new GameObject("RuntimeAudioListener");
            listenerGO.AddComponent<AudioListener>();
            DontDestroyOnLoad(listenerGO);
        }

        Vector3 pos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;

        GameObject tempAudioGO = new GameObject("TempAudio");
        tempAudioGO.transform.position = pos;
        AudioSource aSource = tempAudioGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.ignoreListenerPause = true;
        aSource.Play();
        Destroy(tempAudioGO, clip.length);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(restartSceneIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame(bool status)
    {
        PauseScreen.SetActive(status);
        Time.timeScale = status ? 0f : 1f;
    }

    public void OpenSettings()
    {
        PauseScreen.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
        PauseScreen.SetActive(true);
    }
}
