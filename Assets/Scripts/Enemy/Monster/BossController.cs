using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    [Header("Attack")]
    public float attackRange = 2f;
    public float cleaveDamage = 2f;
    public float timeBetweenAttacks = 3f;

    private float attackCooldown;

    private Transform player1;
    private Transform player2;
    private Animator anim;
    private bool isDead = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player1 = GameObject.FindGameObjectWithTag("Player1")?.transform;
        player2 = GameObject.FindGameObjectWithTag("Player2")?.transform;
    }

    private void Update()
    {
        // تثبیت محور Z
        Vector3 fixedPos = transform.position;
        fixedPos.z = -3f;
        transform.position = fixedPos;

        if (isDead) return;

        attackCooldown -= Time.deltaTime;

        Transform targetPlayer = GetClosestPlayer();

        if (targetPlayer != null && Vector2.Distance(transform.position, targetPlayer.position) <= attackRange)
        {
            if (attackCooldown <= 0f)
            {
                anim.SetTrigger("Cleave");
                attackCooldown = timeBetweenAttacks;
            }
            else
            {
                anim.SetBool("Walk", false);
            }
        }
        else
        {
            Patrol();
        }
    }

    private Transform GetClosestPlayer()
    {
        float dist1 = player1 ? Vector2.Distance(transform.position, player1.position) : Mathf.Infinity;
        float dist2 = player2 ? Vector2.Distance(transform.position, player2.position) : Mathf.Infinity;

        return (dist1 < dist2) ? player1 : player2;
    }

    private void Patrol()
    {
        anim.SetBool("Walk", true);

        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    // این تابع توسط Animation Event در انیمیشن Cleave فراخوانی میشه
    public void CleaveAttack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D col in hitPlayers)
        {
            if (col.CompareTag("Player1") || col.CompareTag("Player2"))
            {
                Health playerHealth = col.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(cleaveDamage);
                    Debug.Log("Boss hit " + col.name + " for " + cleaveDamage + " damage!");
                }
            }
        }
    }

    // Gizmo برای نمایش محدوده حمله
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(float amount)
    {
        // در صورت نیاز سیستم آسیب دیدن باس
    }

    public void Die()
    {
        isDead = true;
        anim.SetTrigger("Died");
        // سایر منطق‌ها مثل پاداش، حذف آبجکت و ...
    }
}
