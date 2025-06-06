using UnityEngine;

public class DoorOP : MonoBehaviour
{
    private bool isOpen = false;

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        foreach (var col in GetComponents<Collider2D>())
            col.isTrigger = true;
        Debug.Log("Door opened by KeyTriggerZone!");
    }
}
