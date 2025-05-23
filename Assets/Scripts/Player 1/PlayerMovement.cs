using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float JumpSpeed = 7f;
    [SerializeField] private float ClimbSpeed = 3f;

    private Rigidbody2D body;
    private float moveInput;
    private float verticalInput;
    private Animator anim;
    private bool Grounded;
    private bool isClimbing;
    private bool canClimb;
    private bool climbingStarted; // برای تشخیص اینکه آیا کاربر شروع به بالا رفتن کرده
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();

        // دیباگ وضعیت‌ها
        Debug.Log($"canClimb: {canClimb}, climbingStarted: {climbingStarted}, isClimbing: {isClimbing}, Gravity: {body.gravityScale}");
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (canClimb)
        {
            verticalInput = Input.GetAxisRaw("Vertical");

            // فقط وقتی W/S فشرده شود، شروع به بالا/پایین رفتن کن
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                climbingStarted = true;
            }
        }
        else
        {
            verticalInput = 0f;
            climbingStarted = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Grounded && !isClimbing)
        {
            Jump();
        }

        if (canClimb && climbingStarted && Mathf.Abs(verticalInput) > 0.1f)
        {
            isClimbing = true;
            body.gravityScale = 0f;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            if (isClimbing)
            {
                isClimbing = false;
                body.gravityScale = 1f;
            }
        }
    }

    private void HandleMovement()
    {
        if (isClimbing && canClimb && climbingStarted)
        {
            // فقط حرکت عمودی مجاز است
            body.linearVelocity = new Vector2(0, verticalInput * ClimbSpeed);
        }
        else
        {
            isClimbing = false;
            body.gravityScale = 1f;

            body.linearVelocity = new Vector2(moveInput * Speed, body.linearVelocity.y);

            if (moveInput > 0.01f)
                transform.localScale = Vector3.one;
            else if (moveInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void HandleAnimation()
    {
        anim.SetBool("Run", moveInput != 0 && !isClimbing);
        anim.SetBool("Grounded", Grounded);
        anim.SetBool("Climb", isClimbing && Mathf.Abs(verticalInput) > 0.1f);
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, JumpSpeed);
        Grounded = false;
        anim.SetTrigger("Jump");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
        {
            canClimb = false;
            climbingStarted = false;
            isClimbing = false;
            body.gravityScale = 1f;
        }
    }

    public bool CanAttack()
    {
        return moveInput == 0 && Grounded && !isClimbing;
    }
}