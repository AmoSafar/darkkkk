using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string currentSceneName;

    public List<PlayerData> players = new List<PlayerData>();
    public List<EnemyData> enemies = new List<EnemyData>();
    public List<ChunkData> chunks = new List<ChunkData>();
}

[System.Serializable]
public class PlayerData
{
    public string playerID;
    public float posX, posY;
    public float health;
}

[System.Serializable]
public class EnemyData
{
    public string enemyType;
    public float posX, posY;
    public float health;
}

[System.Serializable]
public class ChunkData
{
    public string chunkType;
    public float posX, posY;
}
