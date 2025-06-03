using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Ranges")]
    public float chaseRange = 5f;
    public float sprintRange = 3f;
    public float attackRange = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float sprintSpeed = 4f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool hasJumped = false;

    private enum State { Idle, Patrol, Chase, JumpBeforeSprint, Sprint, Attack, Hurt, Die }
    private State currentState;

    [Header("Animation")]
    [SerializeField] private EnemyAnimationManager animManager;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackHitRange = 1f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer <= sprintRange)
        {
            if (!hasJumped)
                currentState = State.JumpBeforeSprint;
            else
                currentState = State.Sprint;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Idle;
        }

        HandleState();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        isGrounded = hit.collider != null;
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                animManager.SetWalking(false);
                animManager.SetAttacking(false);
                break;

            case State.Chase:
                animManager.SetWalking(true);
                animManager.SetAttacking(false);
                MoveTowardsPlayer(moveSpeed);
                break;

            case State.JumpBeforeSprint:
                if (isGrounded)
                {
                    animManager.TriggerJumpStart();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    hasJumped = true;
                }
                break;

            case State.Sprint:
                animManager.SetWalking(true);
                animManager.SetAttacking(false);
                MoveTowardsPlayer(sprintSpeed);
                break;

            case State.Attack:
                animManager.SetWalking(false);
                animManager.SetAttacking(true);
                break;

            case State.Hurt:
                animManager.TriggerHurt();
                break;

            case State.Die:
                animManager.TriggerDie();
                break;
        }
    }

    private void MoveTowardsPlayer(float speed)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void OnHurt()
    {
        currentState = State.Hurt;
    }

    public void OnDie()
    {
        currentState = State.Die;
    }

    /// <summary>
    /// Called by animation event to deal damage to player(s)
    /// </summary>
    public void TryDealDamageToPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackHitRange, playerLayer);

        foreach (Collider2D hit in hits)
        {
            Health health1 = hit.GetComponent<Health>();
            if (health1 != null)
            {
                health1.TakeDamage(attackDamage);
                Debug.Log("Player 1 hit by enemy!");
            }

            Health2 health2 = hit.GetComponent<Health2>();
            if (health2 != null)
            {
                health2.TakeDamage(attackDamage);
                Debug.Log("Player 2 hit by enemy!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackHitRange);
        }
    }
}
