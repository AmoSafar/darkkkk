using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private float lifeTime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // 🔍 دیباگ: اسم GameObject برای فهمیدن کجا این اسکریپت قرار دارد
        Debug.Log($"Projectile attached to: {name}");
    }

    void Update()
    {
        if (hit) return;

        // حرکت تیر
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        // زمان زنده ماندن تیر
        lifeTime += Time.deltaTime;
        if (lifeTime > 5f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Enemy"))
        {
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("Explode");
        }
    }

    public void SetDirection(float _direction)
    {
        lifeTime = 0f;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        // تنظیم جهت صحیح تیر
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    // این متد از انیمیشن "Explode" فراخوانی می‌شود
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
}