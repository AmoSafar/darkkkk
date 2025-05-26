using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 10f;
    [SerializeField] private float _Damage = 1f;
    public float currentHealth { get; private set; }

    public float maxHealth => startingHealth;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage()
    {
        currentHealth = Mathf.Clamp(currentHealth - _Damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            Debug.Log("Player damaged. Current health: " + currentHealth);
        }
        else
        {
            Debug.Log("Player died.");
            // مرگ پلیر
        }
    }

    private void Update()
    {
        // فقط برای تست با کلید E
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage();
        }
    }
}
