using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameLoader : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] playerPrefabs;
    public GameObject[] enemyPrefabs;
    public GameObject[] chunkPrefabs;

    private SaveData loadedData;
    private bool isLoadingScene = false;
    private static bool hasLoadedGameOnce = false;

    private void Update()
    {
        // Save دستی با دکمه E
        if (Input.GetKeyDown(KeyCode.E))
        {
            SaveSystem.SaveGame();
            Debug.Log("Manual Save triggered (E key).");
        }
    }

    public void StartLoadAfterIDsReady()
    {
        if (!hasLoadedGameOnce && SaveSystem.HasSave())
        {
            hasLoadedGameOnce = true;
            LoadGame();
        }
    }

    public void LoadGame()
    {
        if (isLoadingScene) return;

        loadedData = SaveSystem.LoadGame();
        if (loadedData == null) return;

        isLoadingScene = true;
        SceneManager.sceneLoaded += OnSceneLoadedLoadGame;
        SceneManager.LoadScene(loadedData.currentSceneName);
    }

    private void OnSceneLoadedLoadGame(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedLoadGame;

        if (loadedData == null)
        {
            isLoadingScene = false;
            return;
        }

        // ---------- Load Chunks ----------
        foreach (var old in GameObject.FindGameObjectsWithTag("Chunk"))
            Destroy(old);

        foreach (var c in loadedData.chunks)
        {
            var prefab = chunkPrefabs.FirstOrDefault(p => p.GetComponent<Chunk>()?.chunkType == c.chunkType);
            if (prefab != null)
                Instantiate(prefab, new Vector2(c.posX, c.posY), Quaternion.identity);
        }

        // ---------- Load Players ----------
        foreach (var pd in loadedData.players)
        {
            var prefab = playerPrefabs.FirstOrDefault(p => p.GetComponent<Player>().playerID == pd.playerID);
            if (prefab == null) continue;

            GameObject playerObj = Instantiate(prefab, Vector2.zero, Quaternion.identity);

            // غیرفعال کردن Movement قبل از ست کردن Position
            var movement = playerObj.GetComponent<PlayerMovementTopDown>();
            if (movement != null) movement.enabled = false;

            // ست کردن Position و Rigidbody
            var rb = playerObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = new Vector2(pd.posX, pd.posY);
                rb.Sleep(); rb.WakeUp();
            }
            else
            {
                playerObj.transform.position = new Vector2(pd.posX, pd.posY);
            }

            // ست کردن Health
            var healthComp = playerObj.GetComponent<Health>();
            if (healthComp != null) healthComp.SetHealth(pd.health);

            // فعال کردن Movement بعد از Load
            if (movement != null) movement.enabled = true;
        }

        // ---------- Load Enemies ----------
        foreach (var old in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(old);

        foreach (var e in loadedData.enemies)
        {
            var prefab = enemyPrefabs.FirstOrDefault(p => p.GetComponent<Enemy>()?.enemyType == e.enemyType);
            if (prefab == null) continue;

            GameObject enemyObj = Instantiate(prefab, new Vector2(e.posX, e.posY), Quaternion.identity);

            var healthComp = enemyObj.GetComponent<Health>();
            if (healthComp != null) healthComp.SetHealth(e.health);

            var rb = enemyObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = new Vector2(e.posX, e.posY);
                rb.Sleep(); rb.WakeUp();
            }

            var enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null) enemyScript.enabled = true;
        }

        isLoadingScene = false;
        Debug.Log("Game Loaded Successfully!");
    }
}
