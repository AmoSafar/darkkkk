using UnityEngine;
using UnityEngine.InputSystem;

public class SecondPlayerMovement : MonoBehaviour
{
    private PlayerInputActions1 inputActions;
    private Vector2 moveInput;
    private float verticalInput;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float climbSpeed = 3f;

    private bool isGrounded = false;
    private bool isClimbing = false;
    private bool canClimb = false;
    private bool climbingStarted = false;

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
    verticalInput = moveInput.y;
    HandleClimbingInput();

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
        if (isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetTrigger("Jump");
        }
    }

    private void Attack()
    {
        if (!CanAttack()) return;
        anim.SetTrigger("Attack");
        // اینجا کد واقعی حمله یا شلیک را بنویس
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
        anim.SetBool("Climb", isClimbing && Mathf.Abs(verticalInput) > 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.collider.CompareTag("Ground") && !isClimbing)
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
        Debug.Log("Entered ladder zone.");
    }
}


    private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.CompareTag("Ladder"))
    {
        canClimb = false;
        isClimbing = false;
        rb.gravityScale = 1f;
        isGrounded = false; // بازیکن الان روی زمین نیست، باید صبر کنیم تا بخوره به Ground
    }
}

}
