using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRadius = 1.5f;
    public float attackDamage = 2f;
    public Vector2 attackOffset = new Vector2(1f, 0); // جلو دشمن

    [Header("Layer Settings")]
    public LayerMask playerLayer;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 attackPosition = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Gizmos.DrawWireSphere(attackPosition, attackRadius);
    }

    // این متد باید در انیمیشن Attack فراخوانی بشه (Animation Event)
    public void DealDamage()
    {
        Vector3 attackPosition = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRadius, playerLayer);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
                Debug.Log($"[EnemyAttack] Damaged {hit.name} for {attackDamage}");
            }
        }
    }
}
