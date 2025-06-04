using UnityEngine;

public class LeverController2D : MonoBehaviour
{
    public ElevatorController2D elevator; // رفرنس به اسکریپت آسانسور

    private bool playerInZone = false;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.P))
        {
            elevator.ToggleMove();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }
}
