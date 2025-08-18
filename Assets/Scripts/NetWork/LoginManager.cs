using Unity.Netcode;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public void StartAsHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton is null! لطفا یک NetworkManager در صحنه اضافه کن.");
            return;
        }

        NetworkManager.Singleton.StartHost();

        // فقط وقتی SceneManager وجود داشته باشه صحنه را بارگذاری کن
        if (NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby Online", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("SceneManager در NetworkManager تنظیم نشده است!");
        }
    }

    public void StartAsClient()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton is null! لطفا یک NetworkManager در صحنه اضافه کن.");
            return;
        }

        NetworkManager.Singleton.StartClient();
        // Client خودش صحنه را تغییر نمی‌دهد؛ وقتی Host تغییر دهد، Client سینک می‌شود
    }
}
