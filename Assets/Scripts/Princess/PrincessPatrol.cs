using UnityEngine;

public class PrincessPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 2f;

    private Animator animator;
    private Vector3 target;
    private bool movingToB = true;
    private bool isWaiting = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        target = pointB.position;
    }

    private void Update()
    {
        if (isWaiting) return;

        // حرکت به سمت مقصد
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // اجرای انیمیشن راه رفتن
        animator.SetBool("Walk", true);

        // تغییر جهت و حفظ اندازه
        Vector3 scale = transform.localScale;
        scale.x = target.x > transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        scale.x = Mathf.Sign(scale.x) * 0.6f;
        scale.y = 0.6f;
        scale.z = 0.6f;
        transform.localScale = scale;

        // بررسی رسیدن به نقطه هدف
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            StartCoroutine(WaitBeforeMoveAgain());
        }
    }

    private System.Collections.IEnumerator WaitBeforeMoveAgain()
    {
        isWaiting = true;
        animator.SetBool("Walk", false); // توقف انیمیشن راه رفتن
        yield return new WaitForSeconds(waitTime);

        // تغییر جهت حرکت
        movingToB = !movingToB;
        target = movingToB ? pointB.position : pointA.position;
        isWaiting = false;
    }
}
