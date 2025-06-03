using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class EnemyAnimatorController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Target Detection")]
    [SerializeField] private Transform target; // تعیین پلیر در انسپکتور یا خودکار
    [SerializeField] private float detectionRange = 15f; // محدوده تشخیص

    [Header("Ground Check")]
    public bool isGrounded = true;

    [Header("Attack Settings")]
    private bool isAttacking = false;
    private int attackType = 0; // 0: Slash, 1: Kick, 2: RunSlash, 3: AirSlash

    [Header("Other States")]
    private bool isHurt = false;
    private bool isDead = false;

    [Header("Speed Threshold")]
    [SerializeField] private float speedThreshold = 0.1f;

    private bool isFacingRight = true;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }
    }

    private void Update()
    {
        if (isDead)
        {
            animator.SetBool(AnimationParameters.IsDead, true);
            return;
        }

        // بررسی فاصله با پلیر
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance <= detectionRange)
            {
                // چرخش به سمت پلیر
                float direction = target.position.x - transform.position.x;
                if (direction > 0 && !isFacingRight)
                    Flip(true);
                else if (direction < 0 && isFacingRight)
                    Flip(false);
            }
        }

        animator.SetBool(AnimationParameters.IsGrounded, isGrounded);
        animator.SetFloat(AnimationParameters.Speed, Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool(AnimationParameters.IsHurt, isHurt);

        HandleAttackTransition();
    }

    private void HandleAttackTransition()
    {
        if (isAttacking)
        {
            animator.SetBool(AnimationParameters.IsAttacking, true);
            animator.SetInteger(AnimationParameters.AttackType, attackType);
        }

        if (!isAttacking && animator.GetBool(AnimationParameters.IsAttacking))
        {
            animator.SetBool(AnimationParameters.IsAttacking, false);

            if (isGrounded)
            {
                if (Mathf.Abs(rb.linearVelocity.x) > speedThreshold)
                    animator.Play(AnimationParameters.RunningState);
                else
                    animator.Play(AnimationParameters.IdleState);
            }
            else
            {
                animator.Play(AnimationParameters.FallingState);
            }
        }
    }

    private void Flip(bool faceRight)
    {
        isFacingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? 1 : -1;
        transform.localScale = scale;
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
        if (isDead) return;

        isHurt = true;
        animator.SetBool(AnimationParameters.IsHurt, true);
    }

    public void Recover()
    {
        isHurt = false;
        animator.SetBool(AnimationParameters.IsHurt, false);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool(AnimationParameters.IsDead, true);
    }

}