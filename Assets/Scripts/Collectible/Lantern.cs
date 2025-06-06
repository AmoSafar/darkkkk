using UnityEngine;
using System.Collections;

public class Lantern : MonoBehaviour
{
    [Header("Player Interaction")]
    public string playerTag = "Player";

    [Header("Blink Settings")]
    public float blinkInterval = 0.1f;
    public Color blinkColor = Color.yellow;

    [Header("Floating Settings")]
    public float floatSpeed = 1f;
    public float floatAmount = 0.5f;

    [Header("Glow Settings")]
    public float glowSpeed = 2f;
    public float minAlpha = 0.5f;
    public float maxAlpha = 1f;

    [Header("Timing")]
    public float timeBeforeDestroy = 1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 startPos;
    private bool isDying = false;
    private float floatTimer;

    private LanternUIManager uiManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        startPos = transform.position;
        uiManager = FindFirstObjectByType<LanternUIManager>(); // به‌روزرسانی شده
    }


    private void Update()
    {
        if (!isDying)
        {
            // شناور شدن عمودی
            floatTimer += Time.deltaTime * floatSpeed;
            float offsetY = Mathf.Sin(floatTimer) * floatAmount;
            transform.position = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);

            // درخشش (تغییر آلفا)
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * glowSpeed, 1f));
            Color glowColor = originalColor;
            glowColor.a = alpha;
            spriteRenderer.color = glowColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isDying)
        {
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

        // الان که چشمک زدن تموم شده، آیکون رو نمایش بده
        if (uiManager != null)
        {
            uiManager.ShowLanternOn();
        }

        Destroy(gameObject);
    }
}
