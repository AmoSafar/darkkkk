using UnityEngine;

public class Health2 : MonoBehaviour
{
    [SerializeField] private float startingHealth = 10f;
    [SerializeField] private float _Damage = 1f;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool Dead;
    public float maxHealth => startingHealth;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage()
    {
        currentHealth = Mathf.Clamp(currentHealth - _Damage, 0, startingHealth);

        if (currentHealth > 0)
        {
           anim.SetTrigger("Protect");
        }
        else
        {
            if(!Dead) {
                anim.SetTrigger("Dead");
                GetComponent<PlayerMovement>().enabled = false;
                Dead = true;
            }
        }
    }
}
