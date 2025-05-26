using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private float verticalInput;

    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float climbSpeed = 3f;

    private bool isGrounded = false;
    private bool isClimbing = false;
    private bool canClimb = false;
    private bool climbingStarted = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => TryJump();
        inputActions.Player.Shoot.performed += ctx => OnShoot();
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed -= ctx => TryJump();
        inputActions.Player.Shoot.performed -= ctx => OnShoot();

        inputActions.Player.Disable();
    }

    private void Update()
    {
        HandleClimbingInput();

        if (isClimbing && climbingStarted)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(0, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

            if (moveInput.x > 0.01f)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput.x < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        anim.SetBool("Run", moveInput.x != 0 && !isClimbing);
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("Climb", isClimbing && Mathf.Abs(verticalInput) > 0.1f);
    }

    private void HandleClimbingInput()
    {
        verticalInput = moveInput.y;

        if (canClimb)
        {
            climbingStarted = Mathf.Abs(verticalInput) > 0.1f;

            if (climbingStarted && !isClimbing)
            {
                isClimbing = true;
                Physics2D.IgnoreLayerCollision(
                    LayerMask.NameToLayer("Player"),
                    LayerMask.NameToLayer("Ground"),
                    true
                );
            }
        }
        else
        {
            climbingStarted = false;

            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 1f;

                Physics2D.IgnoreLayerCollision(
                    LayerMask.NameToLayer("Player"),
                    LayerMask.NameToLayer("Ground"),
                    false
                );
            }
        }
    }

    private void TryJump()
    {
        if (isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetTrigger("Jump");
        }
    }

    private void OnShoot()
    {
        if (CanAttack())
        {
            anim.SetTrigger("Shoot");
            // اینجا می‌تونی شلیک واقعی رو اضافه کنی
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            canClimb = false;
            climbingStarted = false;

            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 1f;

                Physics2D.IgnoreLayerCollision(
                    LayerMask.NameToLayer("Player"),
                    LayerMask.NameToLayer("Ground"),
                    false
                );
            }
        }
    }

    public bool CanAttack()
    {
        return isGrounded && moveInput.x == 0 && !isClimbing;
    }
}
