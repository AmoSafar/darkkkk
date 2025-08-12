using System.Collections;
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    [Header("Players")]
    public Transform player1;
    public Transform player2;

    [Header("Movement")]
    [Range(0.5f, 10f)]
    public float stopDistance = 2f;
    public float speed = 2f;

    [Header("Attack")]
    public float cleaveDamage = 3f;
    public float minLockTime = 5f;
    public float maxLockTime = 10f;
    public float cleaveRange = 2f; // محدوده آسیب

    private Transform currentTarget;
    private Animator anim;
    private bool isAttacking = false;

    private int lastTargetIndex = -1;
    private float originalScaleX;

    private void Start()
    {
        anim = GetComponent<Animator>();
        originalScaleX = transform.localScale.x;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (true)
        {
            currentTarget = ChooseNextTarget();

            if (currentTarget == null)
            {
                yield return null;
                continue;
            }

            anim.SetBool("Walk", true);

            while (Vector2.Distance(transform.position, currentTarget.position) > stopDistance)
            {
                Vector2 direction = (currentTarget.position - transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;

                if (direction.x != 0)
                {
                    Vector3 currentScale = transform.localScale;
                    currentScale.x = originalScaleX * Mathf.Sign(direction.x);
                    transform.localScale = currentScale;
                }

                yield return null;
            }

            // رسیدن به هدف — تنظیم موقعیت دقیق روی y و روبرو شدن
            Vector3 pos = transform.position;
            float directionSign = Mathf.Sign(currentTarget.position.x - pos.x);
            pos.x = currentTarget.position.x - directionSign * stopDistance;
            pos.y = currentTarget.position.y + 1f;
            transform.position = pos;

            anim.SetBool("Walk", false);

            float lockTime = Random.Range(minLockTime, maxLockTime);
            yield return AttackTarget(currentTarget, lockTime);
        }
    }

    private IEnumerator AttackTarget(Transform target, float lockDuration)
{
    isAttacking = true;
    float timer = 0f;

    while (timer < lockDuration)
    {
        anim.SetTrigger("Cleave");

        yield return new WaitForSeconds(0.8f); // صبر برای رسیدن به فریم ضربه

        Vector2 hitCenter = (Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * cleaveRange * 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitCenter, cleaveRange * 0.5f);

        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                // دمیج به پلیر یا دشمن
                health.TakeDamage(cleaveDamage);

                // اگه هدف پلیر باشه، دمیج بازتاب به باس وارد میشه
                PlayerIdentifier playerId = hit.GetComponent<PlayerIdentifier>();
                if (playerId != null)
                {
                    Health bossHealth = GetComponent<Health>();
                    if (bossHealth != null)
                    {
                        bossHealth.TakeDamage(cleaveDamage);
                    }
                }
            }
        }

        yield return new WaitForSeconds(1f); // فاصله تا ضربه بعدی
        timer += 1.5f;
    }

    isAttacking = false;
}


    private Transform ChooseNextTarget()
    {
        if (player1 == null && player2 == null) return null;

        if (lastTargetIndex == 0 && player2 != null)
        {
            lastTargetIndex = 1;
            return player2;
        }
        else if (lastTargetIndex == 1 && player1 != null)
        {
            lastTargetIndex = 0;
            return player1;
        }

        float distToP1 = player1 != null ? Vector2.Distance(transform.position, player1.position) : float.MaxValue;
        float distToP2 = player2 != null ? Vector2.Distance(transform.position, player2.position) : float.MaxValue;

        if (distToP1 <= distToP2 && player1 != null)
        {
            lastTargetIndex = 0;
            return player1;
        }
        else if (player2 != null)
        {
            lastTargetIndex = 1;
            return player2;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, stopDistance);

        Gizmos.color = Color.red;
        DrawWireCircle(transform.position, stopDistance);
    }

    private void DrawWireCircle(Vector3 center, float radius)
    {
        int segments = 64;
        float angle = 0f;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

        for (int i = 1; i <= segments; i++)
        {
            angle = i * 2f * Mathf.PI / segments;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 center = (Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * cleaveRange * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, cleaveRange * 0.5f);
    }
}
