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
    Rigidbody2D rb = heldRock.GetComponent<Rigidbody2D>();
    rb.isKinematic = true;
    rb.gravityScale = 0f; // سنگ بدون جاذبه در دست بازیکن
    heldRock.GetComponent<Collider2D>().isTrigger = true;
    nearbyRock = null;
}

void Throw()
{
    heldRock.transform.SetParent(null);
    Rigidbody2D rb = heldRock.GetComponent<Rigidbody2D>();
    rb.isKinematic = false;
    rb.gravityScale = 1f; // جاذبه فعال می‌شود
    heldRock.GetComponent<Collider2D>().isTrigger = false;
    rb.AddForce(transform.right * throwForce);
    heldRock = null;
}

}
