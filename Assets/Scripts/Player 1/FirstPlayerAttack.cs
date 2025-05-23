using System.Collections;
using UnityEngine;

public class FirstPlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float meleeAttackCooldown = 1f;       // Cooldown for melee attacks
    [SerializeField] private float rangedAttackCooldown = 1f;     // Cooldown for ranged attacks
    [SerializeField] private float shootDelay = 0.5f;             // Delay before projectile is fired

    [Header("Arrow Settings")]
    [SerializeField] private Transform arrowPoint;                // Position where arrows are fired from
    [SerializeField] private GameObject[] arrows;                 // Arrow object pool

    private Animator anim;
    private PlayerMovement playerMovement;
    private float meleeCooldownTimer = Mathf.Infinity;
    private float rangedCooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Update cooldown timers
        meleeCooldownTimer += Time.deltaTime;
        rangedCooldownTimer += Time.deltaTime;

        // Melee Attack
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanMeleeAttack())
        {
            Attack();
        }

        // Ranged Attack
        if (Input.GetKeyDown(KeyCode.RightShift) && CanRangedAttack())
        {
            StartCoroutine(ShootWithDelay());
        }
    }

    private bool CanMeleeAttack()
    {
        return meleeCooldownTimer > meleeAttackCooldown && playerMovement.CanAttack();
    }

    private bool CanRangedAttack()
    {
        return rangedCooldownTimer > rangedAttackCooldown && playerMovement.CanAttack();
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        meleeCooldownTimer = 0f;
    }

    private IEnumerator ShootWithDelay()
    {
        anim.SetTrigger("Shoot");
        rangedCooldownTimer = 0f;

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