using UnityEngine;

public class Player : MonoBehaviour {
    [Tooltip("شماره بازیکن در صحنه: 1 یا 2 (برای پایدار کردن Player1/Player2)")]
    public int slot = 1;

    [Tooltip("شناسه یکتای ذخیره‌سازی: PlayFabId + _P1/_P2 توسط PlayerIDSetter تنظیم می‌شود")]
    public string playerID;

    [Tooltip("سلامتی بازیکن")]
    public int health = 100;
}
