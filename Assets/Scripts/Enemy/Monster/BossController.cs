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
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player1 = GameObject.FindGameObjectWithTag("Player1")?.transform;
        player2 = GameObject.FindGameObjectWithTag("Player2")?.transform;
    }

    private void Update()
    {
        if (isDead) return;

        // Fix Z
        Vector3 pos = transform.position;
        pos.z = -3f;
        transform.position = pos;

        attackCooldown -= Time.deltaTime;

        Transform targetPlayer = GetClosestPlayer();

        if (targetPlayer != null)
        {
            FlipTowards(targetPlayer);

            float distance = Vector2.Distance(transform.position, targetPlayer.position);
            if (distance <= attackRange)
            {
                anim.SetBool("Walk", false);

                if (attackCooldown <= 0f && !isAttacking)
                {
                    anim.SetTrigger("Cleave");
                    isAttacking = true;
                    attackCooldown = timeBetweenAttacks;
                }
            }
            else
            {
                anim.SetBool("Walk", true);
                MoveToward(targetPlayer);
            }
        }
        else
        {
            Patrol();
        }
    }

    private void FlipTowards(Transform target)
    {
        if (target != null)
        {
            float dir = target.position.x - transform.position.x;
            if (dir > 0)
                spriteRenderer.flipX = false;
            else if (dir < 0)
                spriteRenderer.flipX = true;
        }
    }

    private Transform GetClosestPlayer()
    {
        float dist1 = player1 ? Vector2.Distance(transform.position, player1.position) : Mathf.Infinity;
        float dist2 = player2 ? Vector2.Distance(transform.position, player2.position) : Mathf.Infinity;

        if (dist1 < dist2) return player1;
        else return player2;
    }

    private void MoveToward(Transform target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void Patrol()
    {
        anim.SetBool("Walk", true);
        Transform point = patrolPoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, point.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, point.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    // Called at cleave hit moment from Animation Event
    public void CleaveAttack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D col in hitPlayers)
        {
            if (col.CompareTag("Player1") || col.CompareTag("Player2"))
            {
                Health hp = col.GetComponent<Health>();
                if (hp != null)
                {
                    hp.TakeDamage(cleaveDamage);
                    Debug.Log("Boss hit " + col.name + " for " + cleaveDamage + " damage!");
                }
            }
        }
    }

    // Called at end of cleave animation from Animation Event
    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(float amount)
    {
        // Add logic if needed
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Died");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
