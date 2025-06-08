using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpAndThrow : MonoBehaviour
{
    public Transform holdPoint; // جایی که سنگ رو نگه می‌داره
    public float throwForce = 500f;
    public string rockTag = "Rock";

    private GameObject nearbyRock;
    private GameObject heldRock;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(rockTag))
        {
            nearbyRock = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(rockTag))
        {
            if (collision.gameObject == nearbyRock)
                nearbyRock = null;
        }
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (heldRock == null && nearbyRock != null)
            {
                // برداشتن سنگ
                PickUp();
            }
            else if (heldRock != null)
            {
                // پرتاب سنگ
                Throw();
            }
        }
    }

    void PickUp()
    {
        heldRock = nearbyRock;
        heldRock.transform.SetParent(holdPoint);
        heldRock.transform.localPosition = Vector3.zero;
        heldRock.GetComponent<Rigidbody2D>().isKinematic = true;
        heldRock.GetComponent<Collider2D>().enabled = false;
        nearbyRock = null;
    }

    void Throw()
    {
        heldRock.transform.SetParent(null);
        Rigidbody2D rb = heldRock.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.AddForce(transform.right * throwForce); // پرتاب به سمت راست بازیکن
        heldRock.GetComponent<Collider2D>().enabled = true;
        heldRock = null;
    }
}
