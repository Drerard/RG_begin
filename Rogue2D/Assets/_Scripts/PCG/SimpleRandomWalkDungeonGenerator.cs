using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField, Space(5)] protected SimpleRandomWalkData randomWalkParameters;


    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPos = RunRandomWalk(startPos);

        tilemapVisualizer.PaintFloorTiles(floorPos);
        WallGenerator.CreateWalls(floorPos, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(Vector2Int position)
    {
        Vector2Int currentPos = position;
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();

        for (int i = 0; i < randomWalkParameters.iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgoritms.SimpleRandomWalk(currentPos, randomWalkParameters.walkLenght);
            floorPos.UnionWith(path);

            if (randomWalkParameters.startRandomlyEachIteration)
                currentPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
        }
        return floorPos;
    }
    protected HashSet<Vector2Int> RunRandomWalk(Vector2Int position, BoundsInt roomParam, int offset)
    {
        Vector2Int currentPos = position;
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();

        for (int i = 0; i < randomWalkParameters.iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgoritms.SimpleRandomWalk(currentPos, randomWalkParameters.walkLenght, roomParam, offset);
            floorPos.UnionWith(path);

            if (randomWalkParameters.startRandomlyEachIteration)
                currentPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
        }

        return floorPos;
    }
}
