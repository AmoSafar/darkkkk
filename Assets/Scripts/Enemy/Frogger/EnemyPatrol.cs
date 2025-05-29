using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private bool movingLeft = true;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}
