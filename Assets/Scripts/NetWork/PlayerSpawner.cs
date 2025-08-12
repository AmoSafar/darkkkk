using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Editor;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject[] characters;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        foreach (var player in CharacterSelectDisplay.Instance.players)
        {
            var spawnedPlayer = Instantiate(characters[player.characterID], Vector3.zero, quaternion.identity);
            spawnedPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(player.clientID);
        }

    }
}