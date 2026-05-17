using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;

    public TileChance[] tiles;

    public int width = 20;
    public int height = 20;

    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase randomTile = GetRandomTile();

                Vector3Int pos = new Vector3Int(x, y, 0);

                tilemap.SetTile(pos, randomTile);
            }
        }
    }

    TileBase GetRandomTile()
    {
        int total = 0;

        foreach (TileChance t in tiles)
        {
            total += t.chance;
        }

        int randomNumber = Random.Range(0, total);

        int current = 0;

        foreach (TileChance t in tiles)
        {
            current += t.chance;

            if (randomNumber < current)
            {
                return t.tile;
            }
        }

        return null;
    }
}