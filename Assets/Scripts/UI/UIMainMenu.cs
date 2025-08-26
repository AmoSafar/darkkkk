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
    [SerializeField] private AudioClip menuMusic;
    private AudioSource audioSource;

    [Header("UI")]
    [SerializeField] private TMP_Text errorText;

    private void Awake()
    {
        mainMenuScreen.SetActive(true);
        SettingsPanel.SetActive(false);
        Time.timeScale = 1f;

        // Setup audio
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
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        // Offline mode → Lobby (index 1) → go to level_3 (index 4)
        if (currentIndex == 1)
        {
            SceneManager.LoadScene(4, LoadSceneMode.Single);
            return;
        }

        // Online mode → Lobby Online (index 7)
        if (currentIndex == 7)
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                ShowError("❌ Only the host can start the game!");
                return;
            }

            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            if (playerCount < 2)
            {
                ShowError("⚠️ Waiting for the second player to join!");
                return;
            }

            // 🔹 اینجا باید اسم صحنه رو بدی، نه ایندکس
            NetworkManager.Singleton.SceneManager.LoadScene("level_3", LoadSceneMode.Single);
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
            Invoke(nameof(ClearError), 3f); // clears after 3 seconds
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
