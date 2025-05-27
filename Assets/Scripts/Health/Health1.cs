using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 10f;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool Dead;
    public float maxHealth => startingHealth;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Damage");
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

    public void AddHealth(float _Value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _Value, 0, startingHealth);
    }
}
