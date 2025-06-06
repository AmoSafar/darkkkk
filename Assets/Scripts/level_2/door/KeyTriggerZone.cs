using UnityEngine;

public class KeyTriggerZone : MonoBehaviour
{
    public DoorOP targetDoor; // رفرنس به اسکریپت Door روی آبجکت در

    private bool playerInZone = false;
    private PlayerInventory playerInventory;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerInventory = other.GetComponent<PlayerInventory>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            playerInventory = null;
        }
    }

    private void Update()
    {
        if (playerInZone && playerInventory != null)
        {
            if (playerInventory.hasKey && Input.GetKeyDown(KeyCode.O))
            {
                if (targetDoor != null)
                {
                    targetDoor.OpenDoor();
                }
            }
        }
    }
}
