using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
