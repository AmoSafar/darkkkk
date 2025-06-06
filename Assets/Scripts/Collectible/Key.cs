using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour
{
    [Header("Player Interaction")]
    public string playerTag = "Player";

    [Header("Blink Settings")]
    public float blinkInterval = 0.1f;
    public Color blinkColor = Color.yellow;

    [Header("Timing")]
    public float timeBeforeDestroy = 0.7f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isDying = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isDying)
        {
            // فعال کردن کلید در اینونتوری بازیکن
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
                inventory.hasKey = true;

            StartCoroutine(BlinkUntilDestroy());
        }
    }

    private IEnumerator BlinkUntilDestroy()
    {
        isDying = true;
        float elapsed = 0f;
        bool isBlinkOn = false;

        while (elapsed < timeBeforeDestroy)
        {
            isBlinkOn = !isBlinkOn;
            spriteRenderer.color = isBlinkOn ? blinkColor : originalColor;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        Destroy(gameObject);
    }
}
