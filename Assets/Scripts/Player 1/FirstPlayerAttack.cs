using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FirstPlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float shootDelay = 0.5f;

    [Header("Arrow Settings")]
    [SerializeField] private Transform arrowPoint;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private float arrowLifetime = 2f;
    [SerializeField] public int arrowDamage = 1; // ← اضافه شد

    private Animator anim;
    private PlayerMovement playerMovement;

    private float cooldownTimer = Mathf.Infinity;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Shoot.performed += OnShootPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Shoot.performed -= OnShootPerformed;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        if (cooldownTimer > attackCooldown && playerMovement.CanAttack())
        {
            StartCoroutine(ShootWithDelay());
        }
    }

    private IEnumerator ShootWithDelay()
    {
        anim.SetTrigger("Shoot");
        cooldownTimer = 0f;

        yield return new WaitForSeconds(shootDelay);

        int arrowIndex = FindAvailableArrow();

        if (arrowIndex == -1 || arrowPoint == null)
        {
            Debug.LogWarning("Arrow unavailable or ArrowPoint missing.");
            yield break;
        }

        GameObject arrow = arrows[arrowIndex];
        arrow.transform.position = arrowPoint.position;
        arrow.SetActive(true);
        arrow.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        arrow.GetComponent<Projectile>().SetDamage(arrowDamage); // ← اضافه شد

        StartCoroutine(DisableArrowAfterTime(arrow, arrowLifetime));
    }

    private int FindAvailableArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
                return i;
        }
        return -1;
    }

    private IEnumerator DisableArrowAfterTime(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        arrow.SetActive(false);
    }
}
