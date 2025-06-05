using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalForTwoPlayers : MonoBehaviour
{
    private int playersTouched = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // پورتال را فقط برای همین بازیکن غیرفعال کن
            other.gameObject.SetActive(false);

            playersTouched++;

            if (playersTouched >= 2)
            {
                SceneManager.LoadScene("level_2", LoadSceneMode.Single);
            }
        }
    }
}
