using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackSpeed = 1.0f;
    public float attackRangeX = 5.0f;
    public float attackRangeY = 5.0f;

    [Header("Damage Settings")]
    public int damage = 1;
    public float attackRange = 1f;
    public float attackWidth = 1f;
    public LayerMask playerLayer;
    public BoxCollider2D boxCollider;

    [Header("References")]
    public Animator animator;
    private Transform[] players;

    private bool isAttacking = false;
    private Transform targetPlayer;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip firstAttackClip;
    [SerializeField] private AudioClip secondAttackClip;
    [SerializeField] private AudioClip destroyClip;

    private AudioSource audioSource;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player")
                  .Select(go => go.transform)
                  .ToArray();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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

        if (targetPlayer != null)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        }

        animator.SetTrigger("Stand");
        yield return new WaitForSeconds(0.3f);

        animator.SetTrigger("FirstAttack");
        yield return new WaitForSeconds(0.5f);

        animator.SetTrigger("SecondAttack");
        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(1f / attackSpeed);
        isAttacking = false;
    }

    /// <summary>
    /// Called from animation event to damage player
    /// </summary>
    public void DamagePlayer()
    {
        Vector3 boxCenter = boxCollider.bounds.center + transform.right * attackRange * transform.localScale.x;
        Vector3 boxSize = new Vector3(boxCollider.bounds.size.x * attackWidth, boxCollider.bounds.size.y, boxCollider.bounds.size.z);

        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.zero, 0f, playerLayer);
        if (hit.collider != null)
        {
            var health = hit.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    // --- 🔊 متدهای صدا از Animation Events --- //
    public void PlayFirstAttackSound()
    {
        if (firstAttackClip != null && audioSource != null)
            audioSource.PlayOneShot(firstAttackClip);
    }

    public void PlaySecondAttackSound()
    {
        if (secondAttackClip != null && audioSource != null)
            audioSource.PlayOneShot(secondAttackClip);
    }

    // --- 🔥 پخش صدا هنگام Destroy --- //
    private void OnDestroy()
    {
        if (destroyClip != null)
            AudioSource.PlayClipAtPoint(destroyClip, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Vector3 boxCenter = boxCollider.bounds.center + transform.right * attackRange * transform.localScale.x;
        Vector3 boxSize = new Vector3(boxCollider.bounds.size.x * attackWidth, boxCollider.bounds.size.y, boxCollider.bounds.size.z);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
