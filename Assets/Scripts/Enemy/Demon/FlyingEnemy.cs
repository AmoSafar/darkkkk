using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Target Detection")]
    public Vector2 detectionCenter;
    public Vector2 detectionSize = new Vector2(10f, 5f);
    private Transform nearestPlayer;

    [Header("Zigzag Movement")]
    public float moveSpeed = 3f;
    public float zigzagHeight = 2f;
    public float zigzagFrequency = 2f;
    public float horizontalRange = 3f;

    [Header("Attack Settings")]
    public float attackYThreshold = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 2f;

    private Animator animator;
    private Vector2 startPosition;
    private float cooldownTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Update()
    {
        FindNearestPlayer();

        if (nearestPlayer != null && IsPlayerInRange(nearestPlayer.position))
        {
            ZigzagMove();

            if (transform.position.y <= attackYThreshold && cooldownTimer <= 0f)
            {
                Attack();
            }
        }

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p.transform;
            }
        }

        nearestPlayer = closest;
    }

    bool IsPlayerInRange(Vector2 playerPos)
    {
        Vector2 worldCenter = (Vector2)transform.position + detectionCenter;
        Rect detectionRect = new Rect(worldCenter - detectionSize / 2, detectionSize);
        return detectionRect.Contains(playerPos);
    }

    void ZigzagMove()
    {
        float time = Time.time * moveSpeed;

        float newX = startPosition.x + Mathf.PingPong(time, horizontalRange) - (horizontalRange / 2f);
        float newY = startPosition.y + Mathf.Sin(time * zigzagFrequency) * (zigzagHeight * 0.3f);

        Vector2 previousPosition = transform.position;
        transform.position = new Vector2(newX, newY);

        float direction = newX - previousPosition.x;

        // فرض می‌کنیم شکل پیش‌فرض دشمن به سمت چپ است، پس برای حرکت به راست اسکیل x منفی می‌شود
        if (direction > 0.01f)
            transform.localScale = new Vector3(-10f, 10f, 1f);  // نگاه به راست
        else if (direction < -0.01f)
            transform.localScale = new Vector3(10f, 10f, 1f);   // نگاه به چپ
    }

    void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            Fireball fireball = projectile.GetComponent<Fireball>();
            if (fireball != null)
            {
                if (transform.localScale.x < 0) // انمی به سمت راست نگاه می‌کند
                    fireball.SetDirection(Vector2.right);
                else // انمی به سمت چپ نگاه می‌کند
                    fireball.SetDirection(Vector2.left);
            }
        }

        cooldownTimer = attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 worldCenter = Application.isPlaying ? (Vector2)transform.position + detectionCenter : (Vector2)transform.position + detectionCenter;
        Gizmos.DrawWireCube(worldCenter, detectionSize);
    }
}
