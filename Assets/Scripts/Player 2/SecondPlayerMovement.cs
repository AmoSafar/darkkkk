using UnityEngine;
using UnityEngine.InputSystem;

public class SecondPlayerMovement : MonoBehaviour
{
    private PlayerInputActions1 inputActions;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    private bool isGrounded = false;

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
        inputActions.Player2.Jump.performed += ctx => TryJump();
        inputActions.Player2.Attack.performed += ctx => Attack();
    }

    private void OnDisable()
    {
        inputActions.Player2.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player2.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player2.Jump.performed -= ctx => TryJump();
        inputActions.Player2.Attack.performed -= ctx => Attack();
        inputActions.Player2.Disable();
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        anim.SetBool("Run", moveInput.x != 0);
        anim.SetBool("Grounded", isGrounded);

        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void TryJump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetTrigger("Jump");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public bool CanAttack()
    {
        return isGrounded && moveInput.x == 0;
    }

    private void Attack()
    {
        if (!CanAttack()) return;
        anim.SetTrigger("Attack"); // انیمیشن ضربه اجرا می‌شود
    }
}