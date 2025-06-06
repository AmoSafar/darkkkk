using UnityEngine;
using UnityEngine.UI;

public class HealthBar1 : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        // اگر دستی تنظیم نشده باشه، به صورت اتوماتیک پیدا کن
        if (playerHealth == null)
        {
            foreach (var health in FindObjectsByType<Health>(FindObjectsSortMode.None))
            {
                PlayerIdentifier identifier = health.GetComponent<PlayerIdentifier>();
                if (identifier != null && identifier.isPlayerOne) // یا isPlayerTwo برای بازیکن دوم
                {
                    playerHealth = health;
                    break;
                }
            }
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("HealthBar1: PlayerHealth پیدا نشد!");
            return;
        }

        totalHealthBar.fillAmount = 1f;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // روی Canvas یا شیء والد HealthBar بذار
    }


    private void Update()
    {
        if (playerHealth == null) return;

        float fillPercent = playerHealth.currentHealth / playerHealth.maxHealth;
        currentHealthBar.fillAmount = fillPercent;
    }
}
