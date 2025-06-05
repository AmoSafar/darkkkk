using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    private Vector2 direction = Vector2.left;

    [Header("Damage")]
    public float damage = 1f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // نابودی خودکار بعد از زمان مشخص
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    // متد برای تنظیم جهت حرکت تیر از بیرون
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;

        // اگر جهت به راست باشه، مقیاس X مثبت باشه، در غیر این صورت منفی
        if (newDirection.x > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (newDirection.x < 0)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // اگر پلیر دارای کامپوننت Health بود
            Health health1 = collision.GetComponent<Health>();
            // اگر پلیر دارای کامپوننت Health2 بود
            Health2 health2 = collision.GetComponent<Health2>();

            if (health1 != null)
            {
                health1.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (health2 != null)
            {
                health2.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
