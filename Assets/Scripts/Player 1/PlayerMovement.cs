using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float JumpSpeed = 3f;

    private Rigidbody2D body2;
    private float moveInput;
    private Animator anim2;
    private bool Grounded;

    private void Awake()
    {
        body2 = GetComponent<Rigidbody2D>();
        anim2 = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();
    }

    private void HandleInput()
    {
        moveInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        body2.linearVelocity = new Vector2(moveInput * Speed, body2.linearVelocity.y);

        // چرخش پلیر به چپ و راست
        if (moveInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (moveInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleAnimation()
    {
        anim2.SetBool("Run", moveInput != 0);
        anim2.SetBool("Grounded", Grounded);
    }

    private void Jump()
    {
        body2.linearVelocity = new Vector2(body2.linearVelocity.x, JumpSpeed);
        Grounded = false;
        anim2.SetTrigger("Jump");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = true;
        }
    }

    public bool CanAttack()
    {
        return moveInput == 0 && Grounded;
    }
}