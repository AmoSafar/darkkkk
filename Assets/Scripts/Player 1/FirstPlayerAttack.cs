using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    private Animator anim;
    private PlayerMovement PlayerMovement;

    private float cooldownTimer = Mathf.Infinity;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        PlayerMovement = GetComponent<PlayerMovement>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Shoot.performed += OnShootPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Shoot.performed -= OnShootPerformed;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        if (cooldownTimer > attackCooldown)
        {
            anim.SetTrigger("Shoot");
            cooldownTimer = 0f;
        }
    }
}
