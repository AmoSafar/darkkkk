using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuScreen;

    [Header("Settings")]
    [SerializeField] private GameObject SettingsPanel;

    [Header("Audio")]
    [SerializeField] private AudioClip menuMusic; // صدای پس‌زمینه منو
    private AudioSource audioSource;

    [Header("UI")]
    [SerializeField] private TMP_Text errorText; // متن برای نمایش ارور

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
        audioSource.volume = 0.5f;
        audioSource.Play();

        if (errorText != null)
            errorText.text = "";
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
        string currentScene = SceneManager.GetActiveScene().name;

        // حالت آفلاین → مستقیم ببر به level_3
        if (currentScene == "Lobby")
        {
            SceneManager.LoadScene("level_3", LoadSceneMode.Single);
            return;
        }

        // حالت آنلاین → شرط هاست و حداقل ۲ بازیکن
        if (currentScene == "LobbyOnline")
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                ShowError("❌ فقط هاست می‌تواند بازی را شروع کند!");
                return;
            }

            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            if (playerCount < 2)
            {
                ShowError("⚠️ هنوز بازیکن دوم وصل نشده است!");
                return;
            }

            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
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

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            CancelInvoke(nameof(ClearError));
            Invoke(nameof(ClearError), 3f); // بعد ۳ ثانیه پاک میشه
        }
        else
        {
            Debug.LogWarning(message);
        }
    }

    private void ClearError()
    {
        if (errorText != null)
            errorText.text = "";
    }
}
