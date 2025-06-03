using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Attack Settings")]
    public bool isAttacking = false;
    public int attackType = 0; // 0: Slashing, 1: Kicking, 2: RunSlashing, 3: AirSlash

    [Header("Movement Thresholds")]
    public float speedThreshold = 0.1f;

    private void Update()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", speed);

        HandleAttackTransition(speed);
    }

    private void HandleAttackTransition(float speed)
    {
        // اگر هنوز حمله فعال است، مقداردهی لازم رو انجام بده
        if (isAttacking)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetInteger("AttackType", attackType);
        }

        // وقتی انیمیشن حمله تموم شد یا IsAttacking رو false کردیم
        if (!isAttacking && animator.GetBool("IsAttacking"))
        {
            animator.SetBool("IsAttacking", false);

            // تشخیص بازگشت به حالت مناسب
            if (animator.GetBool("IsGrounded"))
            {
                if (speed > speedThreshold)
                    animator.Play("Running");
                else
                    animator.Play("Idle");
            }
            else
            {
                animator.Play("Falling");
            }
        }
    }

    // این متد رو از Animation Event صدا کن
    public void EndAttack()
    {
        isAttacking = false;
    }

    // برای شروع حمله از بیرون صدا زده میشه
    public void StartAttack(int type)
    {
        isAttacking = true;
        attackType = type;
    }
}
