using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class EnemyAIAnimator : MonoBehaviour
{
    [Header("Player References")]
    public Transform player1;
    public Transform player2;

    [Header("Movement Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Ranges")]
    public float patrolSpeed = 2f;
    public float runSpeed = 6f;
    public float jumpRange = 10f;
    public float attackRange = 3f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;

    private Transform closestPlayer;
    private Animator animator;
    private Rigidbody2D rb;

    private int groundLayer;

    private bool isDead = false;
    private bool isChasing = false;
    private bool hasJumped = false;
    private bool isFacingRight = true;

    void Awake()
    {
        TryGetComponent(out animator);
        TryGetComponent(out rb);

        groundLayer = LayerMask.GetMask("Ground");
    }

    void Start()
    {
        if (player1 == null)
            player1 = GameObject.FindGameObjectWithTag("Player1")?.transform;
        if (player2 == null)
            player2 = GameObject.FindGameObjectWithTag("Player2")?.transform;
    }

    void Update()
    {
        if (isDead) return;

        closestPlayer = GetClosestPlayer();
        if (closestPlayer == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, closestPlayer.position);

        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        animator.SetBool("isGrounded", isGrounded);

        LookAtTarget(closestPlayer);

        if (distanceToPlayer <= jumpRange)
        {
            isChasing = true;

            if (!hasJumped && isGrounded)
            {
                animator.SetBool("isJumping", true);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                hasJumped = true;
            }

            if (hasJumped && isGrounded)
            {
                float direction = Mathf.Sign(closestPlayer.position.x - transform.position.x);
                rb.linearVelocity = new Vector2(direction * runSpeed, rb.linearVelocity.y);
                animator.SetBool("isRunning", true);
            }

            animator.SetBool("playerInAttackRange", distanceToPlayer <= attackRange);
        }
        else
        {
            isChasing = false;
            animator.SetBool("isRunning", false);
            animator.SetBool("playerInAttackRange", false);
            animator.SetBool("isJumping", false);
            hasJumped = false;

            Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);

        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);

        if (transform.position.x >= rightPoint.position.x && isFacingRight)
        {
            Flip(false);
        }
        else if (transform.position.x <= leftPoint.position.x && !isFacingRight)
        {
            Flip(true);
        }
    }

    void Flip(bool faceRight)
    {
        isFacingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void LookAtTarget(Transform target)
    {
        float dir = target.position.x - transform.position.x;
        if (dir > 0 && !isFacingRight)
            Flip(true);
        else if (dir < 0 && isFacingRight)
            Flip(false);
    }

    Transform GetClosestPlayer()
    {
        if (player1 == null && player2 == null) return null;

        float dist1 = player1 ? Vector3.Distance(transform.position, player1.position) : Mathf.Infinity;
        float dist2 = player2 ? Vector3.Distance(transform.position, player2.position) : Mathf.Infinity;

        return dist1 < dist2 ? player1 : player2;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
    }

    public void PlayHurt()
    {
        if (isDead) return;
        animator.SetTrigger("isHurt");
    }

    void OnDrawGizmosSelected()
    {
        if (leftPoint == null || rightPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jumpRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}