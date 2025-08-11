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

    [Header("Pause")]
    [SerializeField] private GameObject PauseScreen;

    [Header("Settings")]
    [SerializeField] private GameObject SettingsPanel;

    [Header("Restart Settings")]
    [SerializeField] private int restartSceneIndex = 1;

    private bool gameEnded = false;
    private bool player1Dead = false;
    private bool player2Dead = false;

    private void Awake()
    {
        GameOverScreen.SetActive(false);
        WinScreen.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (!player1Dead && player1Health.currentHealth <= 0)
            player1Dead = true;

        if (!player2Dead && player2Health.currentHealth <= 0)
            player2Dead = true;

        if (!gameEnded && player1Dead && player2Dead)
        {
            gameEnded = true;
            GameOver();
        }

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
        PlayClipIgnorePause(GameOverSound, SoundManager.Instance.SFXVolume);
    }

    public void Win()
    {
        WinScreen.SetActive(true);
        PlayClipIgnorePause(WinSound, SoundManager.Instance.SFXVolume);
    }

    private void PlayClipIgnorePause(AudioClip clip, float volume)
    {
        if (clip == null) return;

        // اطمینان از وجود AudioListener
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
        aSource.ignoreListenerPause = true; // جلوگیری از قطع شدن هنگام Pause
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
