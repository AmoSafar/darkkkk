using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_SideAway : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float speed;
    [SerializeField] private float movementDistance;
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
    if (movingLeft)
    {
        if (transform.position.x > leftEdge)
        {
            transform.position = new UnityEngine.Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else
        {
            movingLeft = false; // ← تغییر جهت
        }
    }
    else
    {
        if (transform.position.x < rightEdge)
        {
            transform.position = new UnityEngine.Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else
        {
            movingLeft = true; // ← تغییر جهت
        }
    }
}

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
