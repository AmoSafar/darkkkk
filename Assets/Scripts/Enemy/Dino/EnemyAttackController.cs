using UnityEngine;
using System.Collections;
using System.Linq; // برای استفاده از LINQ

public class EnemyAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackSpeed = 1.0f;
    public float attackRangeX = 5.0f;
    public float attackRangeY = 5.0f;

    [Header("References")]
    public Animator animator;
    public Transform[] players;

    private bool isAttacking = false;
    private Transform targetPlayer;

    void Start()
    {
        // پیدا کردن تمام پلیرها با تگ "Player"
        players = GameObject.FindGameObjectsWithTag("Player")
                  .Select(go => go.transform)
                  .ToArray();
    }

    void Update()
    {
        if (!isAttacking)
        {
            Transform closest = FindClosestPlayerInRange();
            if (closest != null)
            {
                targetPlayer = closest;
                StartCoroutine(AttackSequence());
            }
        }
    }

    Transform FindClosestPlayerInRange()
    {
        float minDist = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (Transform player in players)
        {
            Debug.Log("Checking player: " + player.name);

            Vector3 diff = player.position - transform.position;
            float dx = Mathf.Abs(diff.x);
            float dy = Mathf.Abs(diff.y);

            if (dx <= attackRangeX && dy <= attackRangeY)
            {
                float distance = diff.sqrMagnitude;
                if (distance < minDist)
                {
                    minDist = distance;
                    closestPlayer = player;
                }
            }
        }

        return closestPlayer;
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // چرخش به سمت پلیر در بازی 2D (فقط با تغییر scale)
        if (targetPlayer != null)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;

            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // اجرای انیمیشن‌ها
        animator.SetTrigger("Stand");
        yield return new WaitForSeconds(0.3f); // زمان تقریبی برای انیمیشن Stand

        animator.SetTrigger("FirstAttack");
        yield return new WaitForSeconds(0.5f); // زمان تقریبی برای FirstAttack

        animator.SetTrigger("SecondAttack");
        yield return new WaitForSeconds(0.5f); // زمان تقریبی برای SecondAttack

        yield return new WaitForSeconds(1f / attackSpeed);
        isAttacking = false;
    }
}
