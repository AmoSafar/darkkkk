using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    private Animator animator;
    private float idleBlinkTimer;

    [Header("Idle Blink Settings")]
    public float idleBlinkIntervalMin = 3f;
    public float idleBlinkIntervalMax = 7f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ResetIdleBlinkTimer();
    }

    private void Update()
    {
        HandleIdleBlink();
    }

    private void HandleIdleBlink()
    {
        // اگر در حال راه رفتن یا حمله باشد، چشمک نزند
        if (animator.GetBool("isWalking") || animator.GetBool("isAttacking")) return;

        idleBlinkTimer -= Time.deltaTime;
        if (idleBlinkTimer <= 0f)
        {
            TriggerBlink();
            ResetIdleBlinkTimer();
        }
    }

    private void ResetIdleBlinkTimer()
    {
        idleBlinkTimer = Random.Range(idleBlinkIntervalMin, idleBlinkIntervalMax);
    }

    // متدهای ست کردن حالت‌ها
    public void SetIdle(bool idle)
    {
        animator.SetBool("isIdle", idle);
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void SetRunning(bool running)
    {
        animator.SetBool("isRunning", running);
    }

    public void SetAttacking(bool attacking)
    {
        animator.SetBool("isAttacking", attacking);
    }

    // متدهای تریگر انیمیشن‌ها
    public void TriggerJump()
    {
        animator.SetTrigger("jumpStart");
    }

    public void TriggerHurt()
    {
        animator.SetTrigger("hurt");
    }

    public void TriggerDie()
    {
        animator.SetTrigger("die");
    }

    public void TriggerBlink()
    {
        animator.SetTrigger("idleBlink");
    }

    public void TriggerSlash()
    {
        animator.SetTrigger("slash");
    }

    public void TriggerKick()
    {
        animator.SetTrigger("kick");
    }
}
