using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    [Header("Player References")]
    public Transform player1;
    public Transform player2;

    [Header("Ranges")]
    public float walkRange = 15f;
    public float jumpRange = 10f;
    public float attackRange = 3f;

    [Header("Blink Settings")]
    public float minBlinkDelay = 3f;
    public float maxBlinkDelay = 7f;

    private Animator animator;
    private Transform closestPlayer;
    private bool hasJumped = false;
    private bool isDead = false;

    void Awake()
    {
        TryGetComponent(out animator);
    }

    void Start()
    {
        // پیدا کردن پلیرها اگر دستی تنظیم نشده باشند
        if (player1 == null)
            player1 = GameObject.FindGameObjectWithTag("Player1")?.transform;
        if (player2 == null)
            player2 = GameObject.FindGameObjectWithTag("Player2")?.transform;

        StartCoroutine(BlinkRoutine());
    }

    void Update()
    {
        if (isDead || (player1 == null && player2 == null)) return;

        closestPlayer = GetClosestPlayer();
        if (closestPlayer == null) return;

        float distance = Vector3.Distance(transform.position, closestPlayer.position);

        LookAtTarget(closestPlayer);

        // تعیین وضعیت حرکت
        bool shouldWalk = distance <= walkRange && distance > jumpRange;
        animator.SetBool("isWalking", shouldWalk);

        // منطق Jump
        if (!hasJumped && distance <= jumpRange)
        {
            animator.SetBool("playerInJumpRange", true);
            hasJumped = true;
        }

        // منطق حمله
        animator.SetBool("playerInAttackRange", distance <= attackRange);
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minBlinkDelay, maxBlinkDelay);
            yield return new WaitForSeconds(delay);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.SetBool("shouldBlink", true);
                yield return null; // یک فریم صبر کن
                animator.SetBool("shouldBlink", false);
            }
        }
    }

    Transform GetClosestPlayer()
    {
        if (player1 == null && player2 == null) return null;

        float dist1 = player1 ? Vector3.Distance(transform.position, player1.position) : Mathf.Infinity;
        float dist2 = player2 ? Vector3.Distance(transform.position, player2.position) : Mathf.Infinity;

        return dist1 < dist2 ? player1 : player2;
    }

    void LookAtTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public void PlayHurt()
    {
        if (isDead) return;
        animator.SetTrigger("isHurt");
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("isDead", true);
        animator.SetBool("isWalking", false);
        animator.SetBool("playerInJumpRange", false);
        animator.SetBool("playerInAttackRange", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, walkRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jumpRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}