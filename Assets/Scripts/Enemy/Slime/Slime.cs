using UnityEngine;

public class Slime : MonoBehaviour
{
    public float idleDuration = 3f;
    public float runDuration = 2f;
    public int runCycles = 2;
    public float runSpeed = 2f;
    public Vector2 runDirection = Vector2.right;

    public float debuffDuration = 2f;
    public float speedDebuffAmount = 0.5f;
    public float jumpDebuffAmount = 0.5f;

    public Animator animator;
    public Rigidbody2D rb;

    private int currentCycle = 0;
    private float stateTimer = 0f;
    private Vector2 originalPosition;

    private enum State { Idle, Running, Dead }
    private State currentState = State.Idle;

    void Start()
    {
        originalPosition = transform.position;
        stateTimer = idleDuration;
        animator.SetBool("isRunning", false);
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Idle:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    currentState = State.Running;
                    animator.SetBool("isRunning", true);
                    stateTimer = runDuration;
                    currentCycle = 0;
                }
                break;

            case State.Running:
                RunCycle();
                break;
        }
    }

    void RunCycle()
    {
        stateTimer -= Time.deltaTime;
        rb.linearVelocity = new Vector2(runDirection.x * runSpeed, rb.linearVelocity.y);

        if (stateTimer <= 0)
        {
            runDirection *= -1;
            currentCycle++;

            if (currentCycle >= runCycles)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isRunning", false);
                currentState = State.Idle;
                stateTimer = idleDuration;
            }
            else
            {
                stateTimer = runDuration;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == State.Dead) return;

        IDebuffable debuffable = other.GetComponent<IDebuffable>();
        if (debuffable != null)
        {
            debuffable.ApplyDebuff(speedDebuffAmount, jumpDebuffAmount, debuffDuration);
        }

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(1f);
            Die();
        }
    }

    void Die()
    {
        currentState = State.Dead;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("isDead");
        this.enabled = false;
    }
}
