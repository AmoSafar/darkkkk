using System.Collections;
using UnityEngine;

public class EnemyRespawner : MonoBehaviour
{
    [System.Serializable]
    public class RespawnEntry
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;
        public float respawnDelay = 60f; // زمان تأخیر بین اسپاون هر دشمن

        [HideInInspector] public GameObject currentInstance;
        [HideInInspector] public bool isRespawning = false;
    }

    public RespawnEntry[] enemies;

    private void Start()
    {
        StartCoroutine(SpawnEnemiesWithDelayChain());
    }

    // اسپاون اولیه با فاصله زمانی مشخص برای هر دشمن
    private IEnumerator SpawnEnemiesWithDelayChain()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            SpawnEnemy(enemies[i]);
            yield return new WaitForSeconds(enemies[i].respawnDelay);
        }
    }

    // بررسی مداوم برای ریسپاون، اما با حفظ ترتیب و فاصله زمانی بین ریسپاون‌ها
    private void Update()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            var entry = enemies[i];
            if (!entry.isRespawning && (entry.currentInstance == null || !entry.currentInstance.activeInHierarchy))
            {
                StartCoroutine(DelayedRespawnSequence(i));
            }
        }
    }

    // اجرای ریسپاون هر دشمن با فاصله زمانی خاص خودش
    private IEnumerator DelayedRespawnSequence(int startIndex)
    {
        for (int i = startIndex; i < enemies.Length; i++)
        {
            var entry = enemies[i];

            if (entry.isRespawning || (entry.currentInstance != null && entry.currentInstance.activeInHierarchy))
                continue;

            entry.isRespawning = true;
            yield return new WaitForSeconds(entry.respawnDelay);
            SpawnEnemy(entry);
            entry.isRespawning = false;
        }
    }

    private void SpawnEnemy(RespawnEntry entry)
    {
        if (entry.enemyPrefab != null && entry.spawnPoint != null)
        {
            entry.currentInstance = Instantiate(entry.enemyPrefab, entry.spawnPoint.position, Quaternion.identity);
        }
    }
}
