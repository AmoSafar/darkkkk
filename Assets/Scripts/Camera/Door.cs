using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousChunk;
    [SerializeField] private Transform nextChunk;
    [SerializeField] private CameraControl cam; // مطمئن شو کلاسش دقیقاً همین اسم رو داره

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.position.x < transform.position.x)
                cam.MoveToNewChunk(nextChunk);
            else
                cam.MoveToNewChunk(previousChunk);
        }
    }
}
