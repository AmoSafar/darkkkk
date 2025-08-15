using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow1 : MonoBehaviour
{
    [SerializeField] private Button[] buttons;                 // گرفتن دکمه‌ها
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private AudioSource audioSource;

    private RectTransform rect;
    private int currentPosition;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        // اگر AudioSource به Inspector داده نشده بود، خودکار اضافه می‌کنیم
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // مطمئن شدن از اینکه حداقل یک دکمه وجود دارد
        if (buttons == null || buttons.Length == 0)
            Debug.LogWarning("No buttons assigned to SelectionArrow!");
    }

    private void Update()
    {
        if (buttons == null || buttons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            ChangePosition(-1);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            ChangePosition(1);

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            Interact();
    }

    private void ChangePosition(int change)
    {
        if (buttons.Length == 0) return;

        currentPosition += change;

        if (change != 0)
            PlaySound(changeSound);

        // حلقه کردن بین دکمه‌ها
        if (currentPosition < 0)
            currentPosition = buttons.Length - 1;
        else if (currentPosition >= buttons.Length)
            currentPosition = 0;

        RectTransform targetRect = buttons[currentPosition].GetComponent<RectTransform>();
        rect.position = new Vector3(rect.position.x, targetRect.position.y, rect.position.z);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private void Interact()
    {
        if (buttons.Length == 0) return;

        PlaySound(interactSound);
        buttons[currentPosition].onClick.Invoke();
    }
}
