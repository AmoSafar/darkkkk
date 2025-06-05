using System.Collections;
using UnityEngine;

public class DemonHealth : MonoBehaviour
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

    private Animator animator;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = startingHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            if (animator != null)
                animator.SetTrigger("Hurt");  // انیمیشن ضربه

            StartCoroutine(FlashRed());
        }
        else
        {
            if (!isDead)
            {
                isDead = true;

                if (animator != null)
                    animator.SetTrigger("Death");  // انیمیشن مرگ

                StartCoroutine(FlashAndDestroy());
            }
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
