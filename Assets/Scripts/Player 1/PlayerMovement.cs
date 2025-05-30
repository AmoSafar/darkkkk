using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private float verticalInput;

    private Rigidbody2D rb;
    private Animator anim;
    private Health health;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float climbSpeed = 3f;

    private bool isGrounded = false;
    private bool isClimbing = false;
    private bool canClimb = false;
    private bool climbingStarted = false;
    private bool atLadderTop = false;
    private bool lockAtLadderTop = false;

    // اضافه شده: ذخیره آخرین محل برخورد با زمین
    private Vector3 lastGroundedPosition;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        lastGroundedPosition = transform.position;
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
        if (health.currentHealth <= 0) return;

        HandleLadderLockLogic();
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

    private void HandleLadderLockLogic()
    {
        verticalInput = moveInput.y;
        float horizontalInput = moveInput.x;

        if (lockAtLadderTop)
        {
            if (verticalInput < -0.1f)
            {
                lockAtLadderTop = false;
                atLadderTop = false;
                canClimb = true;
                isClimbing = true;
                rb.gravityScale = 0f;
            }
            else if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                lockAtLadderTop = false;
                rb.gravityScale = 1f;
            }
            else
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                isGrounded = true;
                canClimb = false;
                isClimbing = false;
                anim.SetBool("Run", false);
                anim.SetBool("Climb", false);
                anim.SetBool("Grounded", true);
                return;
            }
        }
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
        if (isGrounded && !isClimbing && health.currentHealth > 0)
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            lastGroundedPosition = transform.position; // ذخیره آخرین محل برخورد با زمین
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
        else if (collision.CompareTag("LadderExit"))
        {
            atLadderTop = true;
            lockAtLadderTop = true;
            isClimbing = false;
            canClimb = false;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector2(transform.position.x, collision.transform.position.y);
            isGrounded = true;
        }
        else if (collision.CompareTag("Dead"))
        {
            if (health.currentHealth <= 0) return;

            health.TakeDamage(1f);

            if (health.currentHealth > 0)
            {
                // بازگرداندن پلیر به آخرین محل برخورد با زمین
                transform.position = lastGroundedPosition;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            else
            {
                rb.simulated = false;
            }
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
        else if (collision.CompareTag("LadderExit"))
        {
            atLadderTop = false;
            lockAtLadderTop = false;
        }
    }

    public bool CanAttack()
    {
        return isGrounded && moveInput.x == 0 && !isClimbing && health.currentHealth > 0;
    }
}
