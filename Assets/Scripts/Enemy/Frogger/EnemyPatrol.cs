using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private AudioClip alertClip;

    private bool movingLeft = true;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // برای پخش مداوم
        audioSource.clip = alertClip;
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftPoint.position.x)
            {
                Move(-1);
            }
            else
            {
                movingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightPoint.position.x)
            {
                Move(1);
            }
            else
            {
                movingLeft = true;
            }
        }
    }

    private void Move(int direction)
    {
        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
        spriteRenderer.flipX = direction < 0;
        if (animator != null)
            animator.Play("Move");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
