using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FirstPlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;       // Cooldown برای حمله کلی
    [SerializeField] private float shootDelay = 0.5f;         // تاخیر شلیک

    [Header("Arrow Settings")]
    [SerializeField] private Transform arrowPoint;            // نقطه شلیک تیر
    [SerializeField] private GameObject[] arrows;             // آرایه تیرها

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

        if (arrowIndex == -1)
        {
            Debug.LogWarning("No available arrows.");
            yield break;
        }

        if (arrowPoint == null)
        {
            Debug.LogError("ArrowPoint is not assigned.");
            yield break;
        }

        arrows[arrowIndex].transform.position = arrowPoint.position;
        arrows[arrowIndex].SetActive(true);
        arrows[arrowIndex].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
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
}
