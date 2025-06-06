using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 10f;
    public float currentHealth { get; private set; }
    public float maxHealth => startingHealth;

    private Animator anim;
    private bool Dead;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRend;

    [Header("iFrames")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private int numberOfFlashes = 5;

    [HideInInspector] public Vector3 lastSafePlatformPos;

    private PlayerIdentifier playerIdentifier;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        lastSafePlatformPos = transform.position;
        playerIdentifier = GetComponent<PlayerIdentifier>();
         DontDestroyOnLoad(gameObject); // استفاده زودهنگام
    }

    private void Start()
    {
        if (HealthManager.Instance != null && playerIdentifier != null)
        {
            currentHealth = playerIdentifier.isPlayerOne
                ? HealthManager.Instance.player1Health
                : HealthManager.Instance.player2Health;

            Debug.Log($"[Start] Player {(playerIdentifier.isPlayerOne ? "1" : "2")} Loaded Health: {currentHealth}");
        }
        else
        {
            currentHealth = startingHealth;
            Debug.LogWarning("[Start] HealthManager or PlayerIdentifier missing. Using default health.");
        }
    }

    public void TakeDamage(float amount)
    {
        if (Dead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        Debug.Log($"[TakeDamage] Player {(playerIdentifier.isPlayerOne ? "1" : "2")} took {amount} damage. Health now: {currentHealth}");

        SyncWithHealthManager();

        if (currentHealth > 0)
        {
            anim.SetTrigger(playerIdentifier.isPlayerOne ? "Damage" : "Protect");
            StartCoroutine(Invulnerability());
        }
        else if (!Dead)
        {
            anim.SetTrigger("Dead");

            if (playerIdentifier.isPlayerOne)
                GetComponent<PlayerMovement>().enabled = false;
            else
                GetComponent<SecondPlayerMovement>().enabled = false;

            Dead = true;
        }
    }

    public void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
        Debug.Log($"[AddHealth] Player {(playerIdentifier.isPlayerOne ? "1" : "2")} healed by {value}. Health now: {currentHealth}");

        SyncWithHealthManager();
    }

    public void ForceSyncHealthToManager()
    {
        SyncWithHealthManager(); // فقط زمان تغییر صحنه یا نیاز خاص
    }

    public void RespawnAtLastSafePos()
    {
        Vector3 respawnPos = lastSafePlatformPos + Vector3.down * 0.5f;
        transform.position = respawnPos;

        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(invulnerabilityDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(invulnerabilityDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    private void SyncWithHealthManager()
    {
        if (HealthManager.Instance == null || playerIdentifier == null) return;

        if (playerIdentifier.isPlayerOne)
            HealthManager.Instance.player1Health = currentHealth;
        else
            HealthManager.Instance.player2Health = currentHealth;
    }
}
