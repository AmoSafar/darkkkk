using UnityEngine;

public class AutoElevator2D : MonoBehaviour
{
    public Transform topPoint;      // نقطه بالا
    public Transform bottomPoint;   // نقطه پایین
    public float speed = 2f;        // سرعت حرکت

    private bool movingDown = true;

    void FixedUpdate()
    {
        if (movingDown)
        {
            transform.position = Vector2.MoveTowards(transform.position, bottomPoint.position, speed * Time.fixedDeltaTime);
            if (Vector2.Distance(transform.position, bottomPoint.position) < 0.01f)
                movingDown = false;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, topPoint.position, speed * Time.fixedDeltaTime);
            if (Vector2.Distance(transform.position, topPoint.position) < 0.01f)
                movingDown = true;
        }
    }
}
