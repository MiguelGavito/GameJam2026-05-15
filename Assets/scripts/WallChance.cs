using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class WallChance
{
    public TileBase tile;

    [Range(0, 100)]
    public int chance;
}