using UnityEngine;
using UnityEngine.InputSystem;

public class SecondPlayerMovementTopDown : MonoBehaviour
{
    private PlayerInputActions1 inputActions;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float scale = 0.6f;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions1();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player2.Enable();
        inputActions.Player2.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player2.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player2.Attack.performed += ctx => anim.SetTrigger("Attack");
    }

    private void OnDisable()
    {
        inputActions.Player2.Disable();
    }

    private void Start()
    {
        rb.gravityScale = 0f;
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;

        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(scale, scale, 1f);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-scale, scale, 1f);

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        anim.SetBool("Run", isMoving);
        anim.SetBool("Idle", !isMoving);
    }
}
