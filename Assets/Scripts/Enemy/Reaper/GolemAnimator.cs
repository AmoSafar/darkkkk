using UnityEngine;
using System.Collections;

public class GolemAnimator : MonoBehaviour
{
    public Transform player;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Header("Ranges")]
    public float attackRange = 1.5f;
    public float runRange = 6f;
    public float walkRange = 10f;

    [Header("Damage Settings")]
    public float slashDamage = 10f;
    public float kickDamage = 20f;
    public int maxSlashBeforeKick = 3;

    private Animator animator;
    private MonoBehaviour playerHealthScript;
    private bool isHealthType1 = false;

    private float blinkTimer;
    private float nextBlinkTime;

    private bool isAttacking = false;
    private int slashCount = 0;

    private enum State { Idle, Walking, Jumping, Running, Attacking, Hurt, Dying }
    private State currentState = State.Idle;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        FindPlayerHealthComponent();
        SetNextBlinkTime();
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        HandleBlinking();
    }

    // پیدا کردن کامپوننت سلامتی پلیر (مناسب با دو نوع Health)
    private void FindPlayerHealthComponent()
    {
        if (player.TryGetComponent<Health>(out Health h1))
        {
            playerHealthScript = h1;
            isHealthType1 = true;
        }
        else if (player.TryGetComponent<Health2>(out Health2 h2))
        {
            playerHealthScript = h2;
            isHealthType1 = false;
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
                    SetAnimationState(idle: true);
                    if (distanceToPlayer <= walkRange && distanceToPlayer > runRange)
                        currentState = State.Walking;
                    else if (distanceToPlayer <= runRange)
                        currentState = State.Jumping;
                    break;

                case State.Walking:
                    SetAnimationState(idle: false, running: false);
                    MoveTowardsPlayer(walkSpeed);
                    if (distanceToPlayer <= runRange)
                        currentState = State.Jumping;
                    else if (distanceToPlayer > walkRange)
                        currentState = State.Idle;
                    break;

                case State.Jumping:
                    animator.SetTrigger("jump");
                    yield return new WaitForSeconds(0.5f);
                    currentState = State.Running;
                    break;

                case State.Running:
                    SetAnimationState(idle: false, running: true);
                    MoveTowardsPlayer(runSpeed);
                    if (distanceToPlayer <= attackRange)
                        currentState = State.Attacking;
                    else if (distanceToPlayer > runRange)
                        currentState = State.Walking;
                    break;

                case State.Attacking:
                    SetAnimationState(idle: false, running: false);
                    if (distanceToPlayer > attackRange)
                    {
                        currentState = State.Running;
                        isAttacking = false;
                    }
                    else if (!isAttacking)
                    {
                        yield return StartCoroutine(PerformAttack());
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

    IEnumerator PerformAttack()
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

    // حرکت نرم به سمت پلیر و چرخش
    void MoveTowardsPlayer(float speed)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // چرخش نرم به سمت پلیر (Smooth Rotation)
        float targetScaleX = player.position.x > transform.position.x ? 1f : -1f;
        Vector3 currentScale = transform.localScale;
        currentScale.x = Mathf.Lerp(currentScale.x, targetScaleX, Time.deltaTime * 10f);
        transform.localScale = currentScale;
    }

    void DealDamage(float damageAmount)
    {
        if (playerHealthScript == null)
            return;

        if (isHealthType1)
            ((Health)playerHealthScript).TakeDamage(damageAmount);
        else
            ((Health2)playerHealthScript).TakeDamage(damageAmount);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        animator.SetTrigger("hurt");
        currentState = State.Hurt;

        // اگر می‌خوای بلافاصله دشمن بمیره، می‌تونی این خط رو فعال کنی:
        // Die();
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentState = State.Dying;
        animator.SetTrigger("die");
        StopAllCoroutines();
        this.enabled = false;
    }

    // تنظیم وضعیت انیمیشن
    void SetAnimationState(bool idle = false, bool running = false)
    {
        animator.SetBool("isIdle", idle);
        animator.SetBool("isWalking", !idle && !running);
        animator.SetBool("isRunning", running);
    }

    // مدیریت پلک زدن تصادفی در حالت Idle
    void HandleBlinking()
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, walkRange);
    }
}
