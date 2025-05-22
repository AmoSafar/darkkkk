using UnityEngine;

public class SecondPlayerAttack : MonoBehaviour
{
[SerializeField] private float attackCooldown = 1f;  // زمان بین دو حمله
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // اگر کلید Shift چپ زده شد، cooldown تموم شده و پلیر اجازه‌ی حمله داره
        if (Input.GetKeyDown(KeyCode.RightShift) && cooldownTimer > attackCooldown && playerMovement.CanAttack())
        {
            Attack();
        }

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        cooldownTimer = 0f;
    }
}
