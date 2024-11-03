using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPos, TilemapVisualizer tilemapVisualizer)
    {
        HashSet<Vector2Int> basicWallPos = FindWalls(floorPos, Direction2D.aroundDirectionsList);

        PaintWalls(basicWallPos, floorPos, tilemapVisualizer);
    }


    private static HashSet<Vector2Int> FindWalls(HashSet<Vector2Int> floorPos, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPos = new HashSet<Vector2Int>();

        foreach(var position in floorPos)
        {
            foreach(var direction in directionsList)
            {
                Vector2Int neighbourPos = position + direction;

                if (floorPos.Contains(neighbourPos) == false)
                {
                    wallPos.Add(neighbourPos);
                }
            }
        }
        return wallPos;
    }

    private static void PaintWalls(HashSet<Vector2Int> basicWallPos, HashSet<Vector2Int> floorPos, TilemapVisualizer tilemapVisualizer)
    {
        foreach (var wallPos in basicWallPos)
        {
            string neighboursBinaryType = "";

            foreach (var direction in Direction2D.aroundDirectionsList)
            {
                Vector2Int neighbourPos = wallPos + direction;
                if (floorPos.Contains(neighbourPos))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintWallTile(wallPos, neighboursBinaryType);
        }

    }
}
