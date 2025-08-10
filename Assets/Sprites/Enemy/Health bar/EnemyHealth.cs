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

    [Header("Animation")]
    [SerializeField] private Animator anim;

    [Header("Key Spawner")]
    [SerializeField] private KeySpawner keySpawner;

    public System.Action OnHurt;
    public System.Action OnDeath;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => startingHealth;

    private Color originalColor;

    // متغیر برای جلوگیری از اجرای چندباره انیمیشن Dead
    private bool hasTriggeredDeadAnimation = false;

    private void Awake()
    {
        currentHealth = startingHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (anim == null)
            anim = GetComponent<Animator>();

        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            OnHurt?.Invoke();
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

        OnDeath?.Invoke();

        BossFightController boss = GetComponent<BossFightController>();
        if (boss != null)
        {
            boss.StopAllCoroutines();
            boss.enabled = false;
        }

        if (anim != null && !hasTriggeredDeadAnimation)
        {
            anim.SetTrigger("Dead");
            hasTriggeredDeadAnimation = true;
        }

        StartCoroutine(SpawnKeyAfterAnimation(3f)); // زمان انیمیشن Dead
        StartCoroutine(WaitAndDestroy(6f)); // زمان حذف شیء
    }

    private IEnumerator SpawnKeyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (keySpawner != null)
        {
            keySpawner.SpawnKey(transform.position);
        }
        else
        {
            Debug.LogWarning("KeySpawner reference not assigned in EnemyHealth.");
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

    private IEnumerator WaitAndDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
