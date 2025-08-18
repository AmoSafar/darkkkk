using UnityEngine;
using System.IO;

public static class SaveSystem {
    private static string CurrentPlayerId => PlayerPrefs.GetString("LoggedInPlayerID", string.Empty);
    private static string GetPath() => Application.persistentDataPath + "/" + CurrentPlayerId + "_save.json";

    public static bool HasSave() {
        if (string.IsNullOrEmpty(CurrentPlayerId)) return false;
        return File.Exists(GetPath());
    }

    public static void SaveGame() {
        if (string.IsNullOrEmpty(CurrentPlayerId)) {
            Debug.LogWarning("SaveSystem: LoggedInPlayerID خالی است؛ ابتدا Login کنید.");
            return;
        }

        SaveData data = new SaveData();
        data.currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Players
        foreach (var pObj in GameObject.FindGameObjectsWithTag("Player")) {
            var p = pObj.GetComponent<Player>();
            if (p != null) {
                data.players.Add(new PlayerData {
                    playerID = p.playerID,
                    posX = pObj.transform.position.x,
                    posY = pObj.transform.position.y,
                    health = p.health
                });
            }
        }

        // Enemies
        foreach (var eObj in GameObject.FindGameObjectsWithTag("Enemy")) {
            var e = eObj.GetComponent<Enemy>();
            if (e != null) {
                data.enemies.Add(new EnemyData {
                    enemyType = e.enemyType,
                    posX = eObj.transform.position.x,
                    posY = eObj.transform.position.y,
                    health = e.health
                });
            }
        }

        // Chunks (فقط اگر وجود داشته باشد)
        foreach (var cObj in GameObject.FindGameObjectsWithTag("Chunk")) {
            var c = cObj.GetComponent<Chunk>();
            if (c != null) {
                data.chunks.Add(new ChunkData {
                    chunkType = c.chunkType,
                    posX = cObj.transform.position.x,
                    posY = cObj.transform.position.y
                });
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(), json);
        Debug.Log("Saved: " + GetPath());
    }

    public static SaveData LoadGame() {
        if (!HasSave()) return null;
        string json = File.ReadAllText(GetPath());
        var data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("Loaded: " + GetPath());
        return data;
    }
}
