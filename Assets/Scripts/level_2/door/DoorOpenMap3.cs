using UnityEngine;

public class DoorOpenMap3 : MonoBehaviour
{
    private bool isOpen = false;
    public Sprite sprite1;
    public Sprite sprite2;
    

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite1;
    }

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
        spriteRenderer.sprite = sprite2;


        Debug.Log("Door opened by KeyTriggerZone!");
    }
}
