using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovementTopDown : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float baseScale = 0.6f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeightInBlocks = 2f;
    [SerializeField] private float jumpDuration = 0.5f;
    private bool isJumping = false;

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
        inputActions.Player.Shoot.performed += ctx => anim.SetTrigger("Shoot");
        inputActions.Player.Jump.performed += ctx => TryJump();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        rb.gravityScale = 0f;
        transform.localScale = new Vector3(baseScale, baseScale, 1f);
    }

    private void FixedUpdate()
    {
        if (isJumping) return; // وقتی در حال پرش هست، حرکت متوقف میشه

        Vector2 movement = moveInput.normalized * moveSpeed;
        rb.linearVelocity = movement;

        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(baseScale, baseScale, 1f);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-baseScale, baseScale, 1f);

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        anim.SetBool("Run", isMoving);
        anim.SetBool("Idle", !isMoving);
    }

    private void TryJump()
    {
        if (!isJumping)
            StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        isJumping = true;
        anim.SetTrigger("Jump");

        // موقعیت اولیه
        Vector3 startPos = transform.position;
        // مقصد پرش: به اندازه ارتفاع
        Vector3 targetPos = startPos + new Vector3(0f, jumpHeightInBlocks, 0f);

        float elapsedTime = 0f;

        // پرش به بالا
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // مکث کوتاه در بالا (اختیاری)
        yield return new WaitForSeconds(0.1f);

        // برگشت به پایین (اختیاری)
        elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(targetPos, startPos, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;

        isJumping = false;
    }
}
