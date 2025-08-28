using UnityEngine;

public class PlayerIDSetter : MonoBehaviour
{
    private void Start()
    {
        // همه پلیرها رو پیدا کن
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            Debug.LogWarning("No players found in scene!");
            return;
        }

        // پلیر ۱ و پلیر ۲
        if (players.Length > 0)
        {
            var p1 = players[0].GetComponent<Player>();
            if (p1 != null)
            {
                p1.playerID = "Player1";
                var id = p1.GetComponent<PlayerIdentifier>();
                if (id != null) id.isPlayerOne = true;
            }
        }

        if (players.Length > 1)
        {
            var p2 = players[1].GetComponent<Player>();
            if (p2 != null)
            {
                p2.playerID = "Player2";
                var id = p2.GetComponent<PlayerIdentifier>();
                if (id != null) id.isPlayerOne = false;
            }
        }

        Debug.Log("PlayerIDs assigned. Notifying GameLoader to start LoadGame if Save exists.");

        // اطلاع دادن به GameLoader که پلیرها آماده‌اند
        var loader = FindFirstObjectByType<GameLoader>();
        if (loader != null)
        {
            loader.StartLoadAfterIDsReady();
        }
        else
        {
            Debug.LogWarning("GameLoader not found in scene!");
        }
    }
}
