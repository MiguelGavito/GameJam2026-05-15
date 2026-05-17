using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallGenerator : MonoBehaviour
{
    public Tilemap groundTilemap;

    public Tilemap wallTilemap;

    public WallChance[] wallTiles;

    IEnumerator Start()
    {
        yield return null;

        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(pos))
            {
                CreateWall(pos + Vector3Int.up);
                CreateWall(pos + Vector3Int.down);
                CreateWall(pos + Vector3Int.left);
                CreateWall(pos + Vector3Int.right);
            }
        }
    }

    void CreateWall(Vector3Int pos)
    {
        if (!groundTilemap.HasTile(pos)
            && !wallTilemap.HasTile(pos))
        {
            wallTilemap.SetTile(
                pos,
                GetRandomWall()
            );
        }
    }

    TileBase GetRandomWall()
    {
        int total = 0;

        foreach (WallChance wall in wallTiles)
        {
            total += wall.chance;
        }

        int randomNumber =
            Random.Range(0, total);

        int current = 0;

        foreach (WallChance wall in wallTiles)
        {
            current += wall.chance;

            if (randomNumber < current)
            {
                return wall.tile;
            }
        }

        return null;
    }
}