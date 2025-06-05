using UnityEngine;

// روی پورتال
public class Portal : MonoBehaviour
{
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var blocker = other.GetComponent<PlayerPortalBlocker>();
            if (blocker != null && blocker.canTeleport)
            {
                other.transform.position = destination.position;
                blocker.StartPortalCooldown();
            }
        }
    }
}

