using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RockStopper : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool stopped = false;

    [SerializeField] private AudioClip groundHitSound;
    [SerializeField] private AudioClip enemyHitSound;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stopped) return;

        if (collision.collider.CompareTag("G"))
        {
            PlaySound(groundHitSound);
            StopRock();
        }
        else if (collision.collider.CompareTag("D"))
        {
            PlaySound(enemyHitSound);
            StopRock();
        }
    }

    private void StopRock()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;
        stopped = true;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
