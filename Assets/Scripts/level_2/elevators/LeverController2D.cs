using UnityEngine;

public class LeverController2D : MonoBehaviour
{
    public ElevatorController2D elevator;   // رفرنس به اسکریپت آسانسور
    public Sprite sprite1;                  // اسپرایت حالت اول
    public Sprite sprite2;                  // اسپرایت حالت دوم

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
            // تغییر حالت آسانسور
            elevator.ToggleMove();

            // تغییر اسپرایت اهرم
            isFirstSprite = !isFirstSprite;
            spriteRenderer.sprite = isFirstSprite ? sprite1 : sprite2;
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
