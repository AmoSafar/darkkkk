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

        // برگرداندن پرنسس در جهت مناسب
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1); // رو به راست
        else
            transform.localScale = new Vector3(-1, 1, 1); // رو به چپ

        // چک کردن رسیدن به مقصد
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

        // تغییر مقصد
        movingToB = !movingToB;
        target = movingToB ? pointB.position : pointA.position;
        isWaiting = false;
    }
}
