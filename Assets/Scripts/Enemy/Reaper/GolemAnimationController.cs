using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetIdle(bool val) => animator.SetBool("isIdle", val);
    public void SetWalking(bool val) => animator.SetBool("isWalking", val);
    public void SetRunning(bool val) => animator.SetBool("isRunning", val);
    public void SetAttacking(bool val) => animator.SetBool("isAttacking", val);

    public void TriggerJump() => animator.SetTrigger("jump");
    public void TriggerSlash() => animator.SetTrigger("slash");
    public void TriggerKick() => animator.SetTrigger("kick");
    public void TriggerBlink() => animator.SetTrigger("doBlink");
    public void TriggerHurt() => animator.SetTrigger("hurt");
    public void TriggerDie() => animator.SetTrigger("die");
}
