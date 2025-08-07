using UnityEngine;
using System.Collections;
using System.Linq;

public class MonsterAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackDuration = 1.2f;

    [Header("References")]
    public Animator animator;
    public EnemyHealth enemyHealth;

    private Transform targetPlayer;
    private bool isAttacking = false;
    private bool isDead = false;

    private void Start()
    {
        animator.Play("Idle");
        FindNearestPlayer();
    }

    private void Update()
    {
        if (isDead || enemyHealth.CurrentHealth <= 0) return;

        FindNearestPlayer();

        if (targetPlayer != null && !isAttacking)
        {
            float distance = Vector2.Distance(transform.position, targetPlayer.position);

            if (distance > attackRange)
            {
                animator.Play("Walk");

                // ✅ چرخاندن جهت بر اساس حرکت
                Vector3 direction = (targetPlayer.position - transform.position).normalized;
                if (direction.x != 0)
                {
                    Vector3 scale = transform.localScale;
                    scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x); // مثبت برای راست، منفی برای چپ
                    transform.localScale = scale;
                }

                // ✅ حرکت
                transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                StartCoroutine(Attack());
            }
        }
    }


    private IEnumerator Attack()
    {
        isAttacking = true;
        animator.Play("Attack");
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return;

        targetPlayer = players
            .OrderBy(p => Vector2.Distance(transform.position, p.transform.position))
            .First().transform;
    }

    private void OnEnable()
    {
        enemyHealth.OnHurt += HandleHurt;
        enemyHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        enemyHealth.OnHurt -= HandleHurt;
        enemyHealth.OnDeath -= HandleDeath;
    }

    private void HandleHurt()
    {
        if (!isDead)
            animator.Play("Hurt");
    }

    private void HandleDeath()
    {
        if (isDead) return;

        isDead = true;
        animator.Play("Dead");
        StartCoroutine(DisappearAfterDelay(3f));
    }

    private IEnumerator DisappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
