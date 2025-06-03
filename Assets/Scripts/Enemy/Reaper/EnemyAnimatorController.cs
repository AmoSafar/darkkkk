using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public Transform player;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Header("Ranges")]
    public float attackRange = 1.5f;
    public float runRange = 6f;

    [Header("Damage")]
    public float slashDamage = 10f;
    public float kickDamage = 20f;
    public int maxSlashBeforeKick = 3;

    private Animator animator;
    private MonoBehaviour playerHealthScript;
    private bool isHealth1 = false;

    private float blinkTimer;
    private float nextBlinkTime;

    private bool isAttacking = false;
    private int slashCount = 0;

    private enum State { Idle, Running, Attacking, Hurt, Dying }
    private State currentState = State.Idle;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        FindHealthComponent();
        SetNextBlinkTime();
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        HandleBlink();
    }

    private void FindHealthComponent()
    {
        if (player.TryGetComponent<Health>(out Health h1))
        {
            playerHealthScript = h1;
            isHealth1 = true;
        }
        else if (player.TryGetComponent<Health2>(out Health2 h2))
        {
            playerHealthScript = h2;
            isHealth1 = false;
        }
        else
        {
            Debug.LogError("Player does not have Health or Health2 component!");
        }
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            if (isDead)
                yield break;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            switch (currentState)
            {
                case State.Idle:
                    SetAnimState(idle: true);
                    if (distanceToPlayer <= runRange)
                        currentState = State.Running;
                    break;

                case State.Running:
                    SetAnimState(running: true);
                    if (distanceToPlayer <= attackRange)
                    {
                        currentState = State.Attacking;
                        SetAnimState();
                        isAttacking = false;
                    }
                    else if (distanceToPlayer > runRange)
                    {
                        currentState = State.Idle;
                        SetAnimState(idle: true);
                    }
                    else
                    {
                        MoveTowards(player.position, runSpeed);
                    }
                    break;

                case State.Attacking:
                    SetAnimState();
                    if (distanceToPlayer > attackRange)
                    {
                        currentState = State.Running;
                        SetAnimState(running: true);
                        isAttacking = false;
                    }
                    else if (!isAttacking)
                    {
                        yield return StartCoroutine(AttackRoutine());
                    }
                    break;

                case State.Hurt:
                    yield return new WaitForSeconds(0.5f);
                    currentState = State.Idle;
                    break;

                case State.Dying:
                    yield break;
            }

            yield return null;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        slashCount = 0;

        while (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (slashCount < maxSlashBeforeKick)
            {
                animator.SetTrigger("slash");
                yield return new WaitForSeconds(0.3f);
                DealDamage(slashDamage);
                slashCount++;
            }
            else
            {
                animator.SetTrigger("kick");
                yield return new WaitForSeconds(0.3f);
                DealDamage(kickDamage);
                slashCount = 0;
            }

            yield return new WaitForSeconds(0.7f);
        }

        isAttacking = false;
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void DealDamage(float amount)
    {
        if (playerHealthScript == null)
            return;

        if (isHealth1)
            ((Health)playerHealthScript).TakeDamage(amount);
        else
            ((Health2)playerHealthScript).TakeDamage(amount);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        animator.SetTrigger("hurt");
        currentState = State.Hurt;

        // For demo: Reduce health to zero instantly if hit
        Die();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        currentState = State.Dying;
        animator.SetTrigger("die");
        StopAllCoroutines();
        this.enabled = false;
    }

    void SetAnimState(bool idle = false, bool running = false)
    {
        animator.SetBool("isIdle", idle);
        animator.SetBool("isRunning", running);
    }

    void HandleBlink()
    {
        if (currentState == State.Idle)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= nextBlinkTime)
            {
                animator.SetTrigger("doBlink");
                SetNextBlinkTime();
                blinkTimer = 0f;
            }
        }
    }

    void SetNextBlinkTime()
    {
        nextBlinkTime = Random.Range(3f, 7f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, runRange);
    }
}