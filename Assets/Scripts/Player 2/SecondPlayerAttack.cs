using UnityEngine;
using UnityEngine.InputSystem;

public class SecondPlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1f;

    private Animator anim;
    private SecondPlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private PlayerInputActions1 inputActions;
    private MeleeAttack meleeAttack;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<SecondPlayerMovement>();
        inputActions = new PlayerInputActions1();
        meleeAttack = GetComponent<MeleeAttack>();
    }

    private void OnEnable()
    {
        inputActions.Player2.Enable();
        inputActions.Player2.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player2.Attack.performed -= OnAttackPerformed;
        inputActions.Player2.Disable();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (cooldownTimer > attackCooldown && playerMovement.CanAttack())
        {
            anim.SetTrigger("Attack");
            cooldownTimer = 0f;
        }
    }

    // این تابع در انیمیشن Event صدا زده می‌شود
    public void PerformMeleeAttack()
    {
        meleeAttack?.Attack();
    }
}
