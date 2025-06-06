using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private Button[] buttons;                 // ✅ گرفتن دکمه‌ها به‌عنوان ورودی
    [SerializeField] private AudioClip ChangeSound;
    [SerializeField] private AudioClip InteractSound;
    [SerializeField] private AudioSource audioSource;

    private RectTransform rect;
    private int currentPosition;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
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
            PlaySound(ChangeSound);

        if (currentPosition < 0)
            currentPosition = buttons.Length - 1;
        else if (currentPosition >= buttons.Length)
            currentPosition = 0;

        RectTransform targetRect = buttons[currentPosition].GetComponent<RectTransform>();
        rect.position = new Vector3(rect.position.x, targetRect.position.y, rect.position.z); // ✅ هم‌تراز کردن فلش با دکمه
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void Interact()
    {
        PlaySound(InteractSound);
        buttons[currentPosition].onClick.Invoke();
    }
}
