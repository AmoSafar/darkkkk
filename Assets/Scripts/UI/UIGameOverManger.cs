using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOverManger : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private AudioClip GameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject PauseScreen;
    
    private void Awake()
    {
        GameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(PauseScreen.activeInHierarchy)
                PauseGame(false);
            else
                PauseGame(true);

        }
    }

    public void Resume()
    {
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        GameOverScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
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
        if(status)
        Time.timeScale = 0;
        else
        Time.timeScale = 1;
    }
}
