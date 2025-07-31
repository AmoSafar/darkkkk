using UnityEngine;
using System.Collections.Generic;

public class WaterZone : MonoBehaviour
{
    [Tooltip("درصد کاهش سرعت (مثلاً 0.5 یعنی نصف سرعت)")]
    [Range(0f, 1f)]
    public float slowMultiplier = 0.5f;

    private Dictionary<GameObject, float> originalSpeeds = new Dictionary<GameObject, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var identifier = other.GetComponent<PlayerIdentifier>();
        if (identifier == null) return;

        if (identifier.isPlayerOne)
        {
            var movement = other.GetComponent<PlayerMovementTopDown>();
            if (movement != null && !originalSpeeds.ContainsKey(other.gameObject))
            {
                originalSpeeds[other.gameObject] = movement.MoveSpeed;
                movement.MoveSpeed *= slowMultiplier;
            }
        }
        else
        {
            var movement = other.GetComponent<SecondPlayerMovementTopDown>();
            if (movement != null && !originalSpeeds.ContainsKey(other.gameObject))
            {
                originalSpeeds[other.gameObject] = movement.MoveSpeed;
                movement.MoveSpeed *= slowMultiplier;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!originalSpeeds.ContainsKey(other.gameObject)) return;

        var identifier = other.GetComponent<PlayerIdentifier>();
        if (identifier == null) return;

        if (identifier.isPlayerOne)
        {
            var movement = other.GetComponent<PlayerMovementTopDown>();
            if (movement != null)
                movement.MoveSpeed = originalSpeeds[other.gameObject];
        }
        else
        {
            var movement = other.GetComponent<SecondPlayerMovementTopDown>();
            if (movement != null)
                movement.MoveSpeed = originalSpeeds[other.gameObject];
        }

        originalSpeeds.Remove(other.gameObject);
    }
}
