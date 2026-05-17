using UnityEngine;

public class ChunkStructureSpawner : MonoBehaviour
{
    [System.Serializable]
    public class StructureData
    {
        public GameObject prefab;
    }

    [Header("Structures (you choose)")]
    public StructureData[] structures;

    [Header("Map Settings")]
    public float mapSize = 100f;
    public float chunkSize = 25f;

    [Header("Spawn Settings")]
    public float spawnChance = 0.7f;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        int chunks = Mathf.RoundToInt(mapSize / chunkSize);

        Vector3 origin = transform.position;

        for (int x = 0; x < chunks; x++)
        {
            for (int y = 0; y < chunks; y++)
            {
                TrySpawnChunk(x, y, origin);
            }
        }
    }

    void TrySpawnChunk(int x, int y, Vector3 origin)
    {
        if (Random.value > spawnChance)
            return;

        if (structures.Length == 0)
            return;

        StructureData data =
            structures[Random.Range(0, structures.Length)];

        // 👉 SOLO +X +Y (desde el origin del mapa)
        Vector2 chunkPos = new Vector2(
            origin.x + (x * chunkSize),
            origin.y + (y * chunkSize)
        );

        Vector2 spawnPos = chunkPos + new Vector2(
            Random.Range(0f, chunkSize),
            Random.Range(0f, chunkSize)
        );

        Instantiate(data.prefab, spawnPos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;

    float chunks = mapSize / chunkSize;

    Vector3 origin = transform.position;

    for (int x = 0; x < chunks; x++)
    {
        for (int y = 0; y < chunks; y++)
        {
            Vector3 pos = new Vector3(
                origin.x + (x * chunkSize) + chunkSize / 2f,
                origin.y + (y * chunkSize) + chunkSize / 2f,
                0
            );

            Vector3 size = new Vector3(chunkSize, chunkSize, 0);

            Gizmos.DrawWireCube(pos, size);
        }
    }
}
}