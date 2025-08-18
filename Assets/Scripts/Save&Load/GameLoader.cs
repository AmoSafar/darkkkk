using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameLoader : MonoBehaviour {
    [Header("Enemy Prefabs (همه نوع‌ها)")]
    public GameObject[] enemyPrefabs;

    [Header("Chunk Prefabs (فقط سین دینامیک)")]
    public GameObject[] chunkPrefabs;

    [Header("Auto Save")]
    public float autoSaveInterval = 10f;
    public bool autoLoadOnStart = true; // اگر Save موجود است، به محض ورود به سین، لود کن

    private SaveData loadedData;
    private float timer = 0f;

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoadedAutoSave;
    }

    private void Start() {
        if (autoLoadOnStart && SaveSystem.HasSave()) {
            // وقتی از Login وارد اولین سین گیم‌پلی شدی، بطور خودکار به سین سیو شده می‌روی
            LoadGame();
        }
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer >= autoSaveInterval) {
            SaveSystem.SaveGame();
            timer = 0f;
        }
    }

    public void LoadGame() {
        loadedData = SaveSystem.LoadGame();
        if (loadedData != null) {
            SceneManager.sceneLoaded += OnSceneLoadedLoadGame;
            SceneManager.LoadScene(loadedData.currentSceneName);
        }
    }

    private void OnSceneLoadedLoadGame(Scene scene, LoadSceneMode mode) {
        if (loadedData == null) return;

        // 1) چانک‌ها (اگر در صحنه‌ی ذخیره‌شده وجود داشته‌اند)
        if (loadedData.chunks != null && loadedData.chunks.Count > 0) {
            foreach (var old in GameObject.FindGameObjectsWithTag("Chunk")) Destroy(old);
            foreach (var c in loadedData.chunks) {
                var prefab = GetChunkPrefabByType(c.chunkType);
                if (prefab != null)
                    Instantiate(prefab, new Vector2(c.posX, c.posY), Quaternion.identity);
                else
                    Debug.LogWarning("Chunk prefab not found for: " + c.chunkType);
            }
        }

        // 2) بازیکن‌ها
        var players = GameObject.FindGameObjectsWithTag("Player").Select(go => go.GetComponent<Player>()).ToArray();
        foreach (var p in players) {
            var pd = loadedData.players.FirstOrDefault(x => x.playerID == p.playerID);
            if (pd != null) {
                p.transform.position = new Vector2(pd.posX, pd.posY);
                p.health = pd.health;
            }
        }

        // 3) دشمن‌ها
        foreach (var old in GameObject.FindGameObjectsWithTag("Enemy")) Destroy(old);
        foreach (var e in loadedData.enemies) {
            var prefab = GetEnemyPrefabByType(e.enemyType);
            if (prefab != null) {
                var enemyObj = Instantiate(prefab, new Vector2(e.posX, e.posY), Quaternion.identity);
                var en = enemyObj.GetComponent<Enemy>();
                if (en != null) en.health = e.health;
            } else {
                Debug.LogWarning("Enemy prefab not found for: " + e.enemyType);
            }
        }

        SceneManager.sceneLoaded -= OnSceneLoadedLoadGame;
    }

    private void OnSceneLoadedAutoSave(Scene scene, LoadSceneMode mode) {
        // سیو هنگام تعویض صحنه
        SaveSystem.SaveGame();
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoadedAutoSave;
    }

    private GameObject GetEnemyPrefabByType(string type) {
        foreach (var prefab in enemyPrefabs) {
            var e = prefab.GetComponent<Enemy>();
            if (e != null && e.enemyType == type) return prefab;
        }
        return null;
    }

    private GameObject GetChunkPrefabByType(string type) {
        foreach (var prefab in chunkPrefabs) {
            var c = prefab.GetComponent<Chunk>();
            if (c != null && c.chunkType == type) return prefab;
        }
        return null;
    }
}
