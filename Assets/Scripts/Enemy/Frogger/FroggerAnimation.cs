using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAnimationHandler : MonoBehaviour
{
    [Tooltip("Animator دشمن")]
    [SerializeField] private Animator animator;

    private SpriteRenderer spriteRenderer;
    private Vector3 previousPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // تنظیم اولیه (اگر تصویر اصلی به راست نگاه کند)
        spriteRenderer.flipX = false;

        previousPosition = transform.position;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 currentPosition = transform.position;

        if (currentPosition != previousPosition)
        {
            // اگر تا الان انیمیشن متوقف بوده، دوباره شروع کن
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            {
                animator.Play("Move");
            }

            // تشخیص جهت حرکت و فلیپ
            if (currentPosition.x > previousPosition.x)
                spriteRenderer.flipX = false; // حرکت به راست
            else if (currentPosition.x < previousPosition.x)
                spriteRenderer.flipX = true; // حرکت به چپ
        }

        previousPosition = currentPosition;
    }
}