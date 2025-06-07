using UnityEngine;

// روی پورتال
public class Portal : MonoBehaviour
{
    public Transform destination;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip teleportSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var blocker = other.GetComponent<PlayerPortalBlocker>();
            if (blocker != null && blocker.canTeleport)
            {
                // پخش صدای تلپورت
                if (audioSource != null && teleportSound != null)
                {
                    audioSource.PlayOneShot(teleportSound);
                }

                // تلپورت کردن پلیر
                other.transform.position = destination.position;
                blocker.StartPortalCooldown();
            }
        }
    }
}
