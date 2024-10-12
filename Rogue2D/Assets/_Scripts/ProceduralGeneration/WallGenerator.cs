using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPos, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPos = FindWallsInDirections(floorPos, Direction2D.cardinalDirectionsList);

        tilemapVisualizer.PaintWallTiles(basicWallPos);
    }

    public static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPos, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPos = new HashSet<Vector2Int>();

        foreach(var position in floorPos)
        {
            foreach(var direction in directionsList)
            {
                var neighbourPos = position + direction;

                if (floorPos.Contains(neighbourPos) == false)
                {
                    wallPos.Add(neighbourPos);
                }
            }
        }

        return wallPos;
    }
}
