using UnityEngine;

public class DoorOP : MonoBehaviour
{
    private bool isOpen = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        foreach (var col in GetComponents<Collider2D>())
            col.isTrigger = true;

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        Debug.Log("Door opened by KeyTriggerZone!");
    }
}
