using UnityEngine;

public class Enemy_SideAway : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float movementDistance = 5f;

    [Header("Damage Settings")]
    [SerializeField] private float damage = 1f;

    private bool movingLeft;
    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        leftEdge = transform.position.x - movementDistance;
        rightEdge = transform.position.x + movementDistance;
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftEdge)
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }
            else
            {
                movingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightEdge)
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
            else
            {
                movingLeft = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

}
