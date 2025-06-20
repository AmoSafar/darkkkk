using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SecondPlayerMovement : MonoBehaviour, IDebuffable
{
    private PlayerInputActions1 inputActions;
    private Vector2 moveInput;
    private float verticalInput;

    private Rigidbody2D rb;
    private Animator anim;
    private Health health;

    [HideInInspector] public float moveSpeed = 5f;
    [HideInInspector] public float jumpForce = 7f;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private int maxJumps = 2;

    private int jumpCount;

    private bool isGrounded = false;
    private bool isClimbing = false;
    private bool canClimb = false;
    private bool atLadderTop = false;
    private bool lockAtLadderTop = false;

    private Vector3 lastGroundedPosition;

    // متغیرهای دیباف
    private bool isDebuffed = false;
    private float originalMoveSpeed;
    private float originalJumpForce;

    private void Awake()
    {
        inputActions = new PlayerInputActions1();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        lastGroundedPosition = transform.position;
        originalMoveSpeed = moveSpeed;
        originalJumpForce = jumpForce;
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
        if (health.currentHealth <= 0) return;

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
                UpdateAnimations();
                HandleFlip();
                return;
            }
        }

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

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(0, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }

        UpdateAnimations();
        HandleFlip();
    }

    private void HandleClimbingInput()
    {
        if (canClimb && Mathf.Abs(moveInput.y) > 0.1f)
        {
            isClimbing = true;
            isGrounded = false;
        }
        else if (!canClimb || Mathf.Abs(moveInput.y) <= 0.1f)
        {
            isClimbing = false;
        }
    }

    private void TryJump()
    {
        if ((isGrounded || jumpCount < maxJumps) && !isClimbing && health.currentHealth > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump");
            jumpCount++;

            // اطمینان از غیر فعال بودن Grounded
            isGrounded = false;
        }
    }


    private void Attack()
    {
        if (!CanAttack() || health.currentHealth <= 0) return;
        anim.SetTrigger("Attack");
    }

    public bool CanAttack()
    {
        return isGrounded && moveInput.x == 0 && !isClimbing;
    }

    private void HandleFlip()
    {
        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateAnimations()
    {
        anim.SetBool("Run", Mathf.Abs(moveInput.x) > 0.1f && isGrounded && !isClimbing);
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("Climb", isClimbing && Mathf.Abs(moveInput.y) > 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && !isClimbing)
        {
            isGrounded = true;
            jumpCount = 0;
            lastGroundedPosition = transform.position;
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

    // متد دیباف برای کاهش سرعت و پرش به صورت موقت
    public void ApplyDebuff(float speedAmount, float jumpAmount, float duration)
    {
        if (!isDebuffed)
            StartCoroutine(DebuffCoroutine(speedAmount, jumpAmount, duration));
    }

    private IEnumerator DebuffCoroutine(float speedAmount, float jumpAmount, float duration)
    {
        isDebuffed = true;

        moveSpeed *= speedAmount;
        jumpForce *= jumpAmount;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;

        isDebuffed = false;
    }
}
