using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;       // زمان بین دو حمله نزدیک
    [SerializeField] private float shootDelay = 0.5f;          // زمان تاخیر بین فشردن کلید و پرتاب تیر

    [Header("Arrow Settings")]
    [SerializeField] private Transform arrowPoint;             // محل شلیک تیر
    [SerializeField] private GameObject[] Arrows;              // آرایه‌ای از تیرها (Object Pool)

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // حمله نزدیک (با شمشیر یا مشت)
        if (Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer > attackCooldown && playerMovement.CanAttack())
        {
            Attack();
        }

        // حمله دور (با تیر) همراه با delay
        if (Input.GetKeyDown(KeyCode.RightShift) && cooldownTimer > attackCooldown && playerMovement.CanAttack())
        {
            StartCoroutine(ShootWithDelay());
        }
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        cooldownTimer = 0f;
    }

    private IEnumerator ShootWithDelay()
    {
        anim.SetTrigger("Shoot");
        cooldownTimer = 0f;

        yield return new WaitForSeconds(shootDelay); // تاخیر قابل تنظیم

        int arrowIndex = FindAvailableArrow();

        if (arrowIndex == -1) yield break; // اگه تیر آزاد نداریم، خارج شو

        Arrows[arrowIndex].transform.position = arrowPoint.position;
        Arrows[arrowIndex].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    // پیدا کردن اولین تیر غیرفعال
    private int FindAvailableArrow()
    {
        for (int i = 0; i < Arrows.Length; i++)
        {
            if (!Arrows[i].activeInHierarchy)
                return i;
        }
        return -1; // پیدا نشد
    }
}
