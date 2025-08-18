using UnityEngine;

public class Enemy : MonoBehaviour {
    [Tooltip("نوع دشمن برای بازیابی prefab هنگام Load (برای هر prefab مقدار یکتای مناسب بنویس)")]
    public string enemyType;

    [Tooltip("سلامتی دشمن")]
    public int health = 50;
}
