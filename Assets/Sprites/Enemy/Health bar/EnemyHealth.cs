using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int startingHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Flash Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 1.5f;
    [SerializeField] private float flashInterval = 0.1f;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => startingHealth;

    private Color originalColor;

    private void Awake()
    {
        currentHealth = startingHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            // می‌توانید انیمیشن یا افکت ضربه را اینجا اضافه کنید
            StartCoroutine(FlashRed());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }
    }

    private void Die()
    {
        isDead = true;
        // می‌توانید انیمیشن مرگ را اینجا اضافه کنید
        StartCoroutine(FlashAndDestroy());
    }

    private IEnumerator FlashAndDestroy()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }

        Destroy(gameObject);
    }
}
