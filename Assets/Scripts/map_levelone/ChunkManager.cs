using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private float chunkWidth = 10f;

    [Header("References")]
    [SerializeField] private GameObject[] chunkPrefabs;

    private List<GameObject> activeChunks = new List<GameObject>();

    void Start()
    {
        if (chunkPrefabs.Length < 6)
        {
            Debug.LogError("You need at least 6 prefabs: Start, End, and 4 middle chunks.");
            return;
        }

        activeChunks.Clear();

        // 1. Instantiate Start Chunk
        GameObject startChunk = Instantiate(chunkPrefabs[0], transform);
        startChunk.transform.position = Vector3.zero;
        activeChunks.Add(startChunk);

        // 2. Generate a random permutation for the 4 middle chunks
        List<int> middleIndices = new List<int> { 2, 3, 4, 5 };
        ShuffleList(middleIndices);

        for (int i = 0; i < middleIndices.Count; i++)
        {
            GameObject midChunk = Instantiate(chunkPrefabs[middleIndices[i]], transform);
            float xPos = (i + 1) * chunkWidth;
            midChunk.transform.position = new Vector3(xPos, 0, 0);
            activeChunks.Add(midChunk);
        }

        // 3. Instantiate End Chunk
        GameObject endChunk = Instantiate(chunkPrefabs[1], transform);
        float endPos = (middleIndices.Count + 1) * chunkWidth;
        endChunk.transform.position = new Vector3(endPos, 0, 0);
        activeChunks.Add(endChunk);
    }

    // Fisher-Yates Shuffle
    void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
