using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [SerializeField] private GameObject keyPrefab;

    // تابعی که کلید رو در موقعیت مشخص Instantiate می‌کنه
    public void SpawnKey(Vector3 position)
    {
        if (keyPrefab != null)
        {
            Instantiate(keyPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Key prefab is not assigned in KeySpawner!");
        }
    }
}
