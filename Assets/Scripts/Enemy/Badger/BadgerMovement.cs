using UnityEngine;

public class BadgerMovement : MonoBehaviour
{
    [Header("Chase & Attack Settings")]
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float prepareTime = 1f;
    [SerializeField] private float attackRange = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip moveClip; // صدای حالت Move
    private AudioSource audioSource;

    private Transform[] players;
    private Transform targetPlayer;
    private float lastAttackTime;
    private float prepareTimer;
    private Animator anim;

    private enum State { Idle, Prepare, Move, Attack }
    private State currentState = State.Idle;

    private void Start()
    {
        anim = GetComponent<Animator>();

        // گرفتن یا ساختن AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // پیدا کردن تمام پلیرهایی که Health دارند
        Health[] healthComponents = FindObjectsByType<Health>(FindObjectsSortMode.None);
        players = new Transform[healthComponents.Length];
        for (int i = 0; i < healthComponents.Length; i++)
            players[i] = healthComponents[i].transform;
    }

    private void Update()
    {
        targetPlayer = GetClosestPlayer();

        // Flip به سمت پلیر
        if (targetPlayer != null)
        {
            Vector3 scale = transform.localScale;
            if (targetPlayer.position.x < transform.position.x)
                scale.x = Mathf.Abs(scale.x) * -1f;
            else
                scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        switch (currentState)
        {
            case State.Idle:
                anim.Play("Idle");
                if (targetPlayer != null && Vector2.Distance(transform.position, targetPlayer.position) < chaseDistance)
                {
                    currentState = State.Prepare;
                    prepareTimer = prepareTime;
                    anim.Play("Prepare");
                }
                break;

            case State.Prepare:
                prepareTimer -= Time.deltaTime;
                if (prepareTimer <= 0f)
                {
                    currentState = State.Move;
                    anim.Play("Move");

                    // پخش صدای ورود به حالت Move
                    if (moveClip != null && audioSource != null)
                        audioSource.PlayOneShot(moveClip);
                }
                break;

            case State.Move:
                if (targetPlayer == null)
                {
                    currentState = State.Idle;
                    anim.Play("Idle");
                    break;
                }

                float distance = Vector2.Distance(transform.position, targetPlayer.position);
                if (distance > chaseDistance)
                {
                    currentState = State.Idle;
                    anim.Play("Idle");
                }
                else if (distance <= attackRange)
                {
                    currentState = State.Attack;
                    anim.Play("Attack");
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
                }
                break;

            case State.Attack:
                anim.Play("Attack");

                if (targetPlayer != null)
                {
                    float dist = Vector2.Distance(transform.position, targetPlayer.position);
                    if (dist > attackRange)
                    {
                        currentState = State.Move;
                        anim.Play("Move");

                        // پخش دوباره صدا هنگام بازگشت به Move
                        if (moveClip != null && audioSource != null)
                            audioSource.PlayOneShot(moveClip);
                    }
                }
                break;
        }
    }

    private Transform GetClosestPlayer()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            if (player == null) continue;

            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = player;
            }
        }

        return closest;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (currentState != State.Attack) return;

        Health health = other.GetComponent<Health>();
        if (health != null && Time.time - lastAttackTime > attackCooldown)
        {
            health.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}
