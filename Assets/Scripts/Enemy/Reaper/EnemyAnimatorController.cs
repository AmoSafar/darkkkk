using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Ground Check")]
    public bool isGrounded = true;

    [Header("Attack Settings")]
    public bool isAttacking = false;
    public int attackType = 0; // 0: Slash, 1: Kick, 2: RunSlash, 3: AirSlash

    [Header("Other States")]
    public bool isHurt = false;
    public bool isDead = false;

    [Header("Speed Threshold")]
    public float speedThreshold = 0.1f;

    private void Update()
    {
        if (isDead)
        {
            animator.SetBool("IsDead", true);
            return;
        }

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsHurt", isHurt);

        HandleAttackTransition();
    }

    private void HandleAttackTransition()
    {
        if (isAttacking)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetInteger("AttackType", attackType);
        }

        if (!isAttacking && animator.GetBool("IsAttacking"))
        {
            animator.SetBool("IsAttacking", false);

            if (isGrounded)
            {
                if (Mathf.Abs(rb.linearVelocity.x) > speedThreshold)
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

    // Called from animation event
    public void EndAttack()
    {
        isAttacking = false;
    }

    // Call externally to begin attack
    public void StartAttack(int type)
    {
        isAttacking = true;
        attackType = type;
    }

    public void Hurt()
    {
        isHurt = true;
        animator.SetBool("IsHurt", true);
    }

    public void Recover()
    {
        isHurt = false;
        animator.SetBool("IsHurt", false);
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
    }
}
