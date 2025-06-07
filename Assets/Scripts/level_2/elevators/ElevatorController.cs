using UnityEngine;

public class ElevatorController2D : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;

    private bool isMoving = false;
    private bool movingDown = true;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;

    void FixedUpdate()
    {
        if (isMoving)
        {
            if (movingDown)
            {
                transform.position = Vector2.MoveTowards(transform.position, bottomPoint.position, speed * Time.fixedDeltaTime);
                if (IsAtBottom())
                    isMoving = false;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, topPoint.position, speed * Time.fixedDeltaTime);
                if (IsAtTop())
                    isMoving = false;
            }
        }
    }

    public void ToggleMove()
    {
        if (!isMoving && IsAtBottom())
        {
            movingDown = false;
            isMoving = true;
            PlayMoveSound();
        }
        else if (!isMoving && IsAtTop())
        {
            movingDown = true;
            isMoving = true;
            PlayMoveSound();
        }
    }

    private void PlayMoveSound()
    {
        if (audioSource != null && moveSound != null)
        {
            audioSource.clip = moveSound;
            audioSource.Play();
        }
    }

    private bool IsAtBottom()
    {
        return Vector2.Distance(transform.position, bottomPoint.position) < 0.01f;
    }

    private bool IsAtTop()
    {
        return Vector2.Distance(transform.position, topPoint.position) < 0.01f;
    }
}
