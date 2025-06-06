using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Ranges")]
    public float walkRange = 10f;       // شروع راه رفتن وقتی پلیر داخلش باشه
    public float runRange = 6f;         // شروع دویدن
    public float attackRange = 2f;      // شروع حمله

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool hasJumped = false;

    private enum State { Idle, Walk, Run, Jump, Slash, Kick, Hurt, Die }
    private State currentState;

    private float blinkTimer;
    private float nextBlinkTime;

    [Header("Animation")]
    [SerializeField] private EnemyAnimationManager animManager;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackHitRange = 1f;
    [SerializeField] private int slashDamage = 10;
    [SerializeField] private int kickDamage = 20;
    [SerializeField] private int maxSlashBeforeKick = 3;
    [SerializeField] private LayerMask playerLayer;

    private int slashCount = 0;
    private bool isAttacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetNextBlinkTime();
    }

    private void Update()
    {
        if (currentState == State.Die || currentState == State.Hurt)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Slash;
        }
        else if (distanceToPlayer <= runRange)
        {
            if (!hasJumped && isGrounded)
            {
                currentState = State.Jump;
            }
            else
            {
                currentState = State.Run;
            }
        }
        else if (distanceToPlayer <= walkRange)
        {
            currentState = State.Walk;
        }
        else
        {
            currentState = State.Idle;
        }

        HandleState();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        isGrounded = hit.collider != null;

        HandleBlink();
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                animManager.SetIdle(true);
                animManager.SetWalking(false);
                animManager.SetRunning(false);
                animManager.SetAttacking(false);
                break;

            case State.Walk:
                animManager.SetIdle(false);
                animManager.SetWalking(true);
                animManager.SetRunning(false);
                animManager.SetAttacking(false);
                MoveTowardsPlayer(walkSpeed);
                break;

            case State.Run:
                animManager.SetIdle(false);
                animManager.SetWalking(false);
                animManager.SetRunning(true);
                animManager.SetAttacking(false);
                MoveTowardsPlayer(runSpeed);
                break;

            case State.Jump:
                if (isGrounded && !hasJumped)
                {
                    animManager.TriggerJump();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    hasJumped = true;
                }
                break;

            case State.Slash:
                if (!isAttacking)
                    StartCoroutine(AttackSequence());
                break;

            case State.Kick:
                if (!isAttacking)
                    StartCoroutine(KickAttack());
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

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    private void HandleBlink()
    {
        if (currentState == State.Idle)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= nextBlinkTime)
            {
                animManager.TriggerBlink();
                SetNextBlinkTime();
                blinkTimer = 0f;
            }
        }
    }

    private void SetNextBlinkTime()
    {
        nextBlinkTime = Random.Range(3f, 7f);
    }

    private System.Collections.IEnumerator AttackSequence()
    {
        isAttacking = true;
        slashCount = 0;

        while (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            if (slashCount < maxSlashBeforeKick)
            {
                animManager.TriggerSlash();
                yield return new WaitForSeconds(0.3f);
                DealDamage(slashDamage);
                slashCount++;
            }
            else
            {
                animManager.TriggerKick();
                yield return new WaitForSeconds(0.3f);
                DealDamage(kickDamage);
                slashCount = 0;
            }

            yield return new WaitForSeconds(0.7f);
        }

        isAttacking = false;
    }

    private System.Collections.IEnumerator KickAttack()
    {
        isAttacking = true;
        animManager.TriggerKick();
        yield return new WaitForSeconds(0.3f);
        DealDamage(kickDamage);
        isAttacking = false;
    }

    private void DealDamage(int amount)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackHitRange, playerLayer);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(amount);
        }
    }

    public void OnHurt()
    {
        currentState = State.Hurt;
    }

    public void OnDie()
    {
        currentState = State.Die;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackHitRange);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, runRange);
        Gizmos.DrawWireSphere(transform.position, walkRange);
    }
}
