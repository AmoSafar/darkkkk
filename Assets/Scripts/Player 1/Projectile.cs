using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 2f;

    private float direction;
    private bool hit;

    private Vector3 targetPoint;
    private bool useTargetPoint = false;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (hit) return;

        if (useTargetPoint)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, step);

            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        }
    }

    public void SetDirection(float dir)
    {
        direction = dir;
        gameObject.SetActive(true);
        Vector3 localScale = transform.localScale;
        localScale.x = dir < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    public void SetDamage(int value)
    {
        damage = value;
    }

    public void SetTargetPoint(Vector3 target)
    {
        targetPoint = target;
        useTargetPoint = true;
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
