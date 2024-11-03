using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomBasedDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [Space(5)]
    [SerializeField] private int minRoomWidth = 20;
    [SerializeField] private int minRoomHeight = 20;

    [SerializeField] private int dungeonWidth = 200;
    [SerializeField] private int dungeonHeight = 200;

    [SerializeField, Range(0, 10)] private int offset = 1;

    [SerializeField] private bool randomWalkRooms = false;

    [SerializeField, Space(10)] private bool showRoomsCenter = false;


    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        BoundsInt initialSpace = new BoundsInt((Vector3Int)startPos, new Vector3Int(dungeonWidth, dungeonHeight, 0));
        List<BoundsInt> roomsList = ProceduralGenerationAlgoritms.BinarySpacePartitioning(initialSpace, minRoomWidth, minRoomHeight);

        //распределиние типов комнат по roomsList

        HashSet<Vector2Int> floor;
        HashSet<HashSet<Vector2Int>> roomsFloor;

        if (randomWalkRooms)
        {
            floor = CreateRoomsRndWalk(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);
        
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        if (showRoomsCenter)
        {
            HashSet<Vector2Int> roomCentersHash = new HashSet<Vector2Int>(roomCenters);
            tilemapVisualizer.PaintRoomCenterTile(roomCentersHash);
        }
    }


    private HashSet<Vector2Int> CreateRoomsRndWalk(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int i = 0; i < roomsList.Count; i++)
        {
            BoundsInt roomBounds = roomsList[i];
            Vector2Int roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));

            HashSet<Vector2Int> roomFloor = RunRandomWalk(roomCenter, roomBounds, offset);
            foreach (var pos in roomFloor)
            {
                bool fitX = (roomBounds.xMin + offset <= pos.x) && (pos.x <= roomBounds.xMax - offset);
                bool fitY = (roomBounds.yMin + offset <= pos.y) && (pos.y <= roomBounds.yMax - offset);
                if (fitX && fitY)
                {
                    floor.Add(pos);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(pos);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        List<Vector2Int> duplicatedRoomCenters = new List<Vector2Int>(roomCenters);

        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        Vector2Int currentRoomCenter = duplicatedRoomCenters[Random.Range(0, duplicatedRoomCenters.Count)];
        duplicatedRoomCenters.Remove(currentRoomCenter);

        while (duplicatedRoomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, duplicatedRoomCenters);
            duplicatedRoomCenters.Remove(closest);

            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            corridors.UnionWith(newCorridor);

            currentRoomCenter = closest;
        }
        return corridors;
    }
    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;

        foreach (var pos in roomCenters)
        {
            float currentDistance = Vector2.Distance(currentRoomCenter, pos);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = pos;
            }
        }
        return closest;
    }
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int pos = currentRoomCenter;
        corridor.Add(pos);

        while(pos.y != destination.y)
        {
            if(destination.y > pos.y)
            {
                pos += Vector2Int.up;
            }
            else
            {
                pos += Vector2Int.down;
            }
            corridor.Add(pos);
        }
        while (pos.x != destination.x)
        {
            if (destination.x > pos.x)
            {
                pos += Vector2Int.right;
            }
            else
            {
                pos += Vector2Int.left;
            }
            corridor.Add(pos);
        }
        return corridor;
    }
}
