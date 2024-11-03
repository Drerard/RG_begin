using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapVisualizer: MonoBehaviour
{
    #region FIELDS
    [SerializeField] private Tilemap floorTilemap, wallTilemap;

    [SerializeField, Space(5)] private TileBase centerRoomTile;
    [SerializeField, Space(5)] private TileBase[] floorTile;
    [SerializeField, Space(5)] private WallsData[] wallsTile;
    #endregion


    public void PaintRoomCenterTile(HashSet<Vector2Int> centerPos)
    {
        PaintTiles(centerPos, floorTilemap, centerRoomTile);
    }
    public void PaintFloorTiles(HashSet<Vector2Int> floorPos)
    {
        if (floorTile.Length != 0)
            PaintTiles(floorPos, floorTilemap, floorTile);
    }
    public void PaintWallTiles(HashSet<Vector2Int> wallPos)
    {
        if(wallsTile.Length != 0)
            PaintTiles(wallPos, wallTilemap, wallsTile[0].Tile);
    }

    public void PaintWallTile(Vector2Int wallPos, string neighboursBinaryType)
    {
        if (wallsTile.Length != 0)
        {
            int nghTypeAsInt = Convert.ToInt32(neighboursBinaryType, 2);
            TileBase wallTile = wallsTile[0].Tile;

            for (int i = 0; i < wallsTile.Length; i++)
            {
                if(ExistenceCheck(wallsTile[i].existenceMask, neighboursBinaryType) && ((wallsTile[i].absenceMaskInt & nghTypeAsInt) == 0))
                {
                    wallTile = wallsTile[i].Tile;
                    i = wallsTile.Length;
                }
            }

            PaintSingleTile(wallTilemap, wallTile, wallPos);
        }
    }
    private bool ExistenceCheck(string mask, string nghs)
    {
        for (int i = 0; i < mask.Length; i++)
        {
            if ((mask[i] == '1') && (mask[i] != nghs[i]))
                return false;
        }
        return true;
    }


    private void PaintTiles(HashSet<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }
    private void PaintTiles(HashSet<Vector2Int> positions, Tilemap tilemap, TileBase[] tile)
    {
        foreach (var position in positions)
        {
            int randomTile = Random.Range(0, tile.Length);
            PaintSingleTile(tilemap, tile[randomTile], position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
}
