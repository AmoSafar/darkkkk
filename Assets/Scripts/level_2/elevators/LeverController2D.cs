using UnityEngine;

public class LeverController2D : MonoBehaviour
{
    public ElevatorController2D elevator;
    public Sprite sprite1;
    public Sprite sprite2;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip leverSound;

    private bool playerInZone = false;
    private bool isFirstSprite = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite1;
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.P))
        {
            elevator.ToggleMove();

            isFirstSprite = !isFirstSprite;
            spriteRenderer.sprite = isFirstSprite ? sprite1 : sprite2;

            if (audioSource != null && leverSound != null)
            {
                audioSource.PlayOneShot(leverSound);
            }
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
