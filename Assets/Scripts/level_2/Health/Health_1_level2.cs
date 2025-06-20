using System.Collections;
using UnityEngine;

public class Health_1_level2 : MonoBehaviour
{
    [SerializeField] private float startingHealth = 10f;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool Dead;
    public float maxHealth => startingHealth;

    [Header("iFrames")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private int numberOfFlashes = 5;
    [SerializeField] private SpriteRenderer spriteRend;
    private Collider2D playerCollider;

    [HideInInspector] public Vector3 lastSafePlatformPos;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        lastSafePlatformPos = transform.position;
    }

    private void Start()
    {
        var identifier = GetComponent<PlayerIdentifier>();

        if (identifier != null && HealthManager.Instance != null)
        {
            if (identifier.isPlayerOne)
                currentHealth = HealthManager.Instance.player1Health;
            else
                currentHealth = HealthManager.Instance.player2Health;
        }
        else
        {
            currentHealth = startingHealth;
        }

        Debug.Log($"💚 Loaded health for {gameObject.name}: {currentHealth}");
    }

    public void TakeDamage(float amount)
    {
        if (Dead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Damage");
            StartCoroutine(Invulnerability());
        }
        else if (!Dead)
        {
            anim.SetTrigger("Dead");
            GetComponent<PlayerMovement>().enabled = false;
            Dead = true;
        }
    }

    public void RespawnAtLastSafePos()
    {
        Vector3 respawnPos = lastSafePlatformPos + Vector3.up * -0.5f;
        transform.position = respawnPos;
        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
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
}
