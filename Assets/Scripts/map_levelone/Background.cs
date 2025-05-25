using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private GameObject backgroundPrefabA;
    [SerializeField] private GameObject backgroundPrefabB;

    private int chunkCount = 6; // تعداد کل چانک‌ها

    void Start()
    {
        SpawnAllChunks();
    }

    void SpawnAllChunks()
    {
        float positionX = 0f;

        for (int i = 0; i < chunkCount; i++)
        {
            GameObject prefabToUse = (i < 3) ? backgroundPrefabA : backgroundPrefabB;
            GameObject bg = Instantiate(prefabToUse, new Vector3(positionX, 0, 0), Quaternion.identity, transform);

            // گرفتن عرض این بک‌گراند برای قرار دادن بعدی
            SpriteRenderer renderer = bg.GetComponent<SpriteRenderer>();
            float chunkWidth = renderer.bounds.size.x;

            // موقعیت بعدی را به اندازه عرض این چانک جلو می‌بریم
            positionX += chunkWidth;
        }
    }
}
