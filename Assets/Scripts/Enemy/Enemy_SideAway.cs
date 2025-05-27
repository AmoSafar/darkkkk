using UnityEngine;

public class Enemy_SideAway : MonoBehaviour
{
    [SerializeField] private float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // اگر به Player 1 برخورد کرد
        Health health1 = collision.GetComponent<Health>();
        if (health1 != null)
        {
            health1.TakeDamage(damage);
            return;
        }

        // اگر به Player 2 برخورد کرد
        Health2 health2 = collision.GetComponent<Health2>();
        if (health2 != null)
        {
            health2.TakeDamage(damage);
            return;
        }
    }
}
