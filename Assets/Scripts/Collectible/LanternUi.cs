using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LanternUIManager : MonoBehaviour
{
    public Image lanternIcon;
    public float fadeDuration = 1f;

    private void Start()
    {
        if (lanternIcon != null)
        {
            // ابتدا آیکون مخفی باشه
            Color c = lanternIcon.color;
            c.a = 0f;
            lanternIcon.color = c;
            lanternIcon.gameObject.SetActive(true);
        }
    }

    public void ShowLanternOn()
    {
        if (lanternIcon != null)
        {
            StartCoroutine(FadeInLanternIcon());
        }
    }

    private IEnumerator FadeInLanternIcon()
    {
        float elapsed = 0f;
        Color color = lanternIcon.color;

        while (elapsed < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            lanternIcon.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        lanternIcon.color = color;
    }
}
