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

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
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

    public void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
    }

    private IEnumerator Invulnerability()
    {
        // جلوگیری از برخورد با دشمنان در مدت iFrame
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        // Flash کردن بازیکن
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // قرمز نیمه‌شفاف
            yield return new WaitForSeconds(invulnerabilityDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(invulnerabilityDuration / (numberOfFlashes * 2));
        }

        // بازگرداندن برخورد با دشمن
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }
}
