using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 2f;
    private float direction;
    private bool hit;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (hit) return;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    public void SetDirection(float dir)
    {
        direction = dir;
        gameObject.SetActive(true);
        // اگر بخوای جهت گرافیکی تیر هم عوض شه:
        Vector3 localScale = transform.localScale;
        if (dir < 0) localScale.x = -Mathf.Abs(localScale.x);
        else localScale.x = Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            hit = true;
            collision.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
