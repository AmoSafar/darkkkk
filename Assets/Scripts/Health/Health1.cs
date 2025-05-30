using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
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

    // برای Respawn
    [HideInInspector] public Vector3 lastSafePlatformPos;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        lastSafePlatformPos = transform.position;
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
        else
        {
            if (!Dead)
            {
                anim.SetTrigger("Dead");
                GetComponent<PlayerMovement>().enabled = false;
                Dead = true;
            }
        }
    }

    public void RespawnAtLastSafePos()
    {
        // کمی بالاتر از سکو قرار بگیر
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
