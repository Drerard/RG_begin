using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLogicGenerator : AbstractLogicDungeonGenerator
{
    [Space(5)]
    [SerializeField] private SimpleSpawner roomBounds;

    protected override void RunProceduralGeneration()
    {
        GenerateRoomBounds();
    }

    private void GenerateRoomBounds()
    {
        foreach (var room in dungeon.ordinaryRooms)
        {
            SpawnRoomBounds(room);
        }
    }
    private void SpawnRoomBounds(Room room)
    {
        Vector2 spawnPos;
        Vector2 actualSize;
        Transform newGameObject;

        (spawnPos, actualSize) = FindActualRoomSize(room);

        newGameObject = Instantiate(roomBounds.prefab, spawnPos, Quaternion.identity, roomBounds.parent).transform;
        newGameObject.GetComponent<RoomBounds>().SetOwnRoom(room);
        newGameObject.GetComponent<BoxCollider2D>().size = new Vector2(actualSize.x, actualSize.y);

        dungeon.allSpawnedObj.Add(newGameObject);
    }

    private (Vector2 spawnPos, Vector2 actualSize) FindActualRoomSize(Room room)
    {
        Vector2 centerPos = new Vector2();
        Vector2 size = new Vector2();
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var point in room.floor)
        {
            if (point.x < minX)
                minX = point.x;
            if (point.x > maxX)
                maxX = point.x;
            if (point.y < minY)
                minY = point.y;
            if (point.y > maxY)
                maxY = point.y;
        }

        size.x = maxX - minX;
        size.y = maxY - minY;
        centerPos.x = (minX + maxX) / 2f + 0.5f;
        centerPos.y = (minY + maxY) / 2f + 0.5f;

        return (centerPos, size);
    }
}
