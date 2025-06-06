using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSwitch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Light2D[] playerLights = other.GetComponentsInChildren<Light2D>();
            foreach (var light in playerLights)
                light.enabled = !light.enabled; // وضعیت نور را تغییر می‌دهد (خاموش→روشن، روشن→خاموش)
        }
    }
}
