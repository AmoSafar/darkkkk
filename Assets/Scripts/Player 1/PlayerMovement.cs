using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour, IDebuffable
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
    [SerializeField] private float fastRunSpeed = 10f;
    [SerializeField] private float fastRunDuration = 2f;

    private bool isFastRunning = false;

    private bool isGrounded = false;
    private bool isClimbing = false;
    private bool canClimb = false;
    private bool atLadderTop = false;
    private bool lockAtLadderTop = false;

    private Vector3 lastGroundedPosition;

    // کاهش سرعت هنگام برخورد با دشمن
    [SerializeField] private float slowDuration = 1f;
    [SerializeField] private float slowAmount = 0.5f;
    private bool isSlowed = false;
    private float originalMoveSpeed;

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
        originalMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => TryJump();
        inputActions.Player.Shoot.performed += ctx => OnShoot();

        inputActions.Player.Dash.performed += ctx => TryDash();
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed -= ctx => TryJump();
        inputActions.Player.Shoot.performed -= ctx => OnShoot();

        inputActions.Player.Dash.performed -= ctx => TryDash();

        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (health.currentHealth <= 0) return;

        verticalInput = moveInput.y;
        float horizontalInput = moveInput.x;

        // منطق قفل شدن در بالای نردبان (LadderExit)
        if (lockAtLadderTop)
        {
            HandleLadderLockLogic();
            return;
        }

        // منطق خروج از بالای نردبان
        if (atLadderTop)
        {
            if (verticalInput < -0.1f)
            {
                atLadderTop = false;
                canClimb = true;
                isClimbing = true;
            }
            else
            {
                canClimb = false;
                isClimbing = false;
            }
        }
        else
        {
            HandleClimbingInput();
        }

        // حرکت روی نردبان
        if (isClimbing)
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

        // Update Run/Idle animation states
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f && !isClimbing;

        if (!isFastRunning)
        {
            anim.SetBool("Run", isMoving);
            anim.SetBool("Idle", !isMoving);
        }
        else
        {
            anim.SetBool("Run", false);
            anim.SetBool("Idle", false);
        }
    }

    private void HandleLadderLockLogic()
    {
        verticalInput = moveInput.y;
        float horizontalInput = moveInput.x;

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
        }
    }

    private void HandleClimbingInput()
    {
        verticalInput = moveInput.y;

        if (canClimb && Mathf.Abs(verticalInput) > 0.1f)
        {
            isClimbing = true;
            isGrounded = false;
        }
        else if (!canClimb || Mathf.Abs(verticalInput) <= 0.1f)
        {
            isClimbing = false;
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
            lastGroundedPosition = transform.position;
        }

        // اگر به دشمن برخورد کرد، کاهش سرعت بده
        if (collision.collider.CompareTag("Enemy"))
        {
            ApplySlow();
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
            isClimbing = false;
            rb.gravityScale = 1f;
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

    // کاهش سرعت موقت پس از برخورد با دشمن
    public void ApplySlow()
    {
        if (!isSlowed)
            StartCoroutine(SlowCoroutine());
    }

    private IEnumerator SlowCoroutine()
    {
        isSlowed = true;
        moveSpeed *= slowAmount;

        yield return new WaitForSeconds(slowDuration);

        moveSpeed = originalMoveSpeed;
        isSlowed = false;
    }

    public void ApplyDebuff(float speedFactor, float jumpFactor, float duration)
    {
        if (!isSlowed)
            StartCoroutine(DebuffCoroutine(speedFactor, jumpFactor, duration));
    }

    private IEnumerator DebuffCoroutine(float speedFactor, float jumpFactor, float duration)
    {
        isSlowed = true;

        float originalJumpForce = jumpForce;
        moveSpeed *= speedFactor;
        jumpForce *= jumpFactor;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        isSlowed = false;
    }

    private void TryDash()
    {
        if (!isFastRunning && isGrounded && Mathf.Abs(moveInput.x) > 0.1f)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        isFastRunning = true;
        float originalSpeed = moveSpeed;
        float originalAnimSpeed = anim.speed;
        moveSpeed = fastRunSpeed;
        anim.speed = 1.8f;
        anim.SetBool("FastRun", true);

        yield return new WaitForSeconds(fastRunDuration);

        moveSpeed = originalSpeed;
        anim.speed = originalAnimSpeed;
        anim.SetBool("FastRun", false);

        isFastRunning = false;

        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f && !isClimbing;
        anim.SetBool("Run", isMoving);
        anim.SetBool("Idle", !isMoving);

        anim.Play(isMoving ? "Run" : "Idle");
        anim.Update(0f);
    }
}
