using UnityEngine;

public class RockStopper : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool stopped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stopped) return;

        if (collision.collider.CompareTag("G") || collision.collider.CompareTag("D"))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static; // سنگ را کاملاً ثابت می‌کند
            stopped = true;
        }
    }
}
