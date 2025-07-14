using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int startingHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Flash Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private float flashInterval = 0.1f;

    // رویدادها برای هماهنگی با انیمیشن‌ها
    public System.Action OnHurt;
    public System.Action OnDeath;

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
            OnHurt?.Invoke(); // صدا زدن انیمیشن Hurt
            StartCoroutine(FlashRed());
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke(); // صدا زدن انیمیشن Dead
        StartCoroutine(FlashAndDestroy());
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

        // Destroy کردن شیء دشمن بعد از مرگ
        Destroy(gameObject);
    }
}
