using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalForTwoPlayers : MonoBehaviour
{
    private int playersTouched = 0;
    private bool player1Touched = false;
    private bool player2Touched = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerIdentifier identifier = other.GetComponent<PlayerIdentifier>();
            Health health = other.GetComponent<Health>();

            if (identifier == null || health == null)
                return;

            // فقط یک بار برای هر بازیکن
            if (identifier.isPlayerOne && !player1Touched)
            {
                player1Touched = true;
                playersTouched++;
            }
            else if (!identifier.isPlayerOne && !player2Touched)
            {
                player2Touched = true;
                playersTouched++;
            }

            // ذخیره دستی مقدار health قبل از تغییر صحنه
            health.ForceSyncHealthToManager();

            // وقتی هر دو بازیکن وارد پورتال شدند
            if (playersTouched >= 2)
            {
                SceneManager.LoadScene("level_2", LoadSceneMode.Single);
            }

            // اختیاری: مخفی‌سازی بازیکن بعد از ورود
            other.gameObject.SetActive(false);
        }
    }
}
