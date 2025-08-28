using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, GetCurrentPlayerID() + "_save.json");

    // ---------- Save ----------
    public static void SaveGame()
    {
        SaveData data = new SaveData();
        data.currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // --- Save Players ---
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var go in players)
        {
            var p = go.GetComponent<Player>();
            if (p != null)
            {
                PlayerData pd = new PlayerData
                {
                    playerID = p.playerID,
                    posX = p.transform.position.x,
                    posY = p.transform.position.y,
                    health = p.health
                };
                data.players.Add(pd);
            }
        }

        // --- Save Enemies ---
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var go in enemies)
        {
            var e = go.GetComponent<Enemy>();
            if (e != null)
            {
                EnemyData ed = new EnemyData
                {
                    enemyType = e.enemyType,
                    posX = go.transform.position.x,
                    posY = go.transform.position.y,
                    health = e.health
                };
                data.enemies.Add(ed);
            }
        }

        // --- Save Chunks ---
        var chunks = GameObject.FindGameObjectsWithTag("Chunk");
        foreach (var go in chunks)
        {
            var c = go.GetComponent<Chunk>();
            if (c != null)
            {
                ChunkData cd = new ChunkData
                {
                    chunkType = c.chunkType,
                    posX = go.transform.position.x,
                    posY = go.transform.position.y
                };
                data.chunks.Add(cd);
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Game Saved: {SavePath}");
    }

    // ---------- Load ----------
    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Save file not found!");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log($"Loaded: {SavePath}");
        return data;
    }

    // ---------- Check Save Exists ----------
    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    // ---------- Helper ----------
    private static string GetCurrentPlayerID()
    {
        return PlayerPrefs.GetString("LoggedInPlayerID", "OfflinePlayer");
    }
}
