using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement; // ✅ اضافه شد

public class LoginManager : MonoBehaviour
{
    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
        // Client صحنه رو خودش عوض نمی‌کنه،
        // وقتی Host تغییر بده، Client هم به صورت خودکار سینک میشه
    }
}
