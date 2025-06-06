using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class StaticLightBlink : MonoBehaviour
{
    public float offDuration ; // مدت خاموش بودن
    public float onDuration ;  // مدت روشن بودن

    private Light2D staticLight;

    void Start()
    {
        // گرفتن Light2D از child
        staticLight = GetComponentInChildren<Light2D>();
        if (staticLight != null)
            StartCoroutine(BlinkLight());
        else
            Debug.LogWarning("No Light2D found as child of " + gameObject.name);
    }

    IEnumerator BlinkLight()
    {
        while (true)
        {
            staticLight.enabled = false; // خاموش
            yield return new WaitForSeconds(offDuration);

            staticLight.enabled = true;  // روشن
            yield return new WaitForSeconds(onDuration);
        }
    }
}
