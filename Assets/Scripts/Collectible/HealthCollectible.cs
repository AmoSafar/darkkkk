using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue = 1f;
    [SerializeField] private string sortingLayerName = "ForeGround";
    [SerializeField] private int sortingOrder = 10;

    private void Start()
    {
        // اطمینان از اینکه Sprite روی لایه‌ی مناسب هست
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }

        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // بررسی برای هر شی که اسکریپت Health داشته باشه
        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.AddHealth(healthValue);
            gameObject.SetActive(false);
        }
    }
}
