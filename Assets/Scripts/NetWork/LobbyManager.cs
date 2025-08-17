using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    private NetworkVariable<int> readyPlayers = new NetworkVariable<int>(0); // شمارش بازیکن‌های آماده

    public void PressReady()
    {
        if (IsClient)
        {
            SubmitReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        readyPlayers.Value++;
        CheckAllReady();
    }

    private void CheckAllReady()
    {
        if (readyPlayers.Value >= 2) // فرض دو بازیکن
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}
