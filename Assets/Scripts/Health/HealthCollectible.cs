using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue = 1f;

    private void OnEnable()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // بررسی اینکه آیا پلیر دارای Health هست
        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.AddHealth(healthValue);
            gameObject.SetActive(false);
            return;
        }

        // بررسی اینکه آیا پلیر دارای Health2 هست
        Health2 health2 = collision.GetComponent<Health2>();
        if (health2 != null)
        {
            health2.AddHealth(healthValue);
            gameObject.SetActive(false);
        }
    }
}
