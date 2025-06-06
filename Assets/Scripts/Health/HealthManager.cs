using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    public float player1Health = 10f;
    public float player2Health = 10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // حفظ اطلاعات بین صحنه‌ها
    }
}
