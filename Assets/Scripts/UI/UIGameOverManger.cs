using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOverManger : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private AudioClip GameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject PauseScreen;

    [Header("Settings")]
    [SerializeField] private GameObject SettingsPanel;

    [Header("Restart Settings")]
    [Tooltip("Index of the scene to restart from (e.g. 1 for Map1, 2 for Map2)")]
    [SerializeField] private int restartSceneIndex = 1;

    private void Awake()
    {
        GameOverScreen.SetActive(false);
        SettingsPanel.SetActive(false); // مخفی کردن پنل تنظیمات در ابتدا
    }

    private void Update()
    {
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

        if (GameOverSound != null)
        {
            AudioSource.PlayClipAtPoint(GameOverSound, Camera.main.transform.position, SoundManager.Instance.SFXVolume);
        }
    }


    public void Restart()
    {
        Time.timeScale = 1f; // اطمینان از اینکه بازی از حالت pause خارج شده
        SceneManager.LoadScene(restartSceneIndex); // بارگذاری سین مشخص‌شده
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // فرض بر این است که منوی اصلی ایندکس 0 دارد
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
