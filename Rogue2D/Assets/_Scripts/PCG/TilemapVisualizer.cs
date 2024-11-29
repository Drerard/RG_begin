using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapVisualizer: MonoBehaviour
{
    #region FIELDS
    [SerializeField] private Tilemap floorTilemap, wallTilemap, corridorTilemap, decorTilemap;
    [Space(5)]
    [SerializeField] private TileBase centerRoomTile;
    [SerializeField] private TileBase corridorTile;
    [Space(5)]
    [SerializeField] private TileBase[] floorTile;
    [SerializeField] private WallsData[] wallsTile;


    [HideInInspector] public List<Vector2Int> topWalls;
    #endregion

    public void PaintDecorTile(Vector2Int centerPos, TileBase tile)
    {
        PaintSingleTile(decorTilemap, tile, centerPos);
    }

    public void PaintRoomCenterTile(Vector2Int centerPos)
    {
        PaintSingleTile(floorTilemap, centerRoomTile, centerPos);
    }
    public void PaintCorridorTile(Vector2Int centerPos)
    {
        PaintSingleTile(corridorTilemap, corridorTile, centerPos);
    }
    public void PaintFloorTiles(HashSet<Vector2Int> floorPos)
    {
        if (floorTile.Length != 0)
            PaintTiles(floorTilemap, floorTile, floorPos);
    }
    public void PaintWallTiles(HashSet<Vector2Int> wallPos)
    {
        if(wallsTile.Length != 0)
            PaintTiles(wallTilemap, wallsTile[0].Tile, wallPos);
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
                    if (wallsTile[i].wallName == "Top")
                        topWalls.Add(wallPos);

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


    private void PaintTiles(Tilemap tilemap, TileBase tile, HashSet<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }
    private void PaintTiles(Tilemap tilemap, TileBase[] tile, HashSet<Vector2Int> positions)
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
        corridorTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        decorTilemap.ClearAllTiles();
    }
}
