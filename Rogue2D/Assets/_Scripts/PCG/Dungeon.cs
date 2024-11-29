using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public TilemapVisualizer tilemapVisualizer = null;

    [HideInInspector] public Room startRoom = null;
    [HideInInspector] public Room endRoom = null;
    [HideInInspector] public Room treasureRoom = null;
    [HideInInspector] public List<Room> ordinaryRooms = new List<Room>();
    [HideInInspector] public List<List<Vector2Int>> allCorridors;
    [HideInInspector] public List<Transform> allSpawnedObj = new List<Transform>();
    [HideInInspector] public List<Transform> allSpawnedChest = new List<Transform>();


    public void AssignKeyRooms(List<Room> allRooms, BoundsInt initialSpace, bool placeStartRoomInCorner)
    {
        //start room
        Room farthestRoom = null;
        float maxDistance = 0;
        if (placeStartRoomInCorner)
        {
            Vector2Int initialSpaceCenter = (Vector2Int)Vector3Int.RoundToInt(initialSpace.center);
            foreach (var room in allRooms)
            {
                float currentDistance = Vector2.Distance(initialSpaceCenter, room.center);
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    farthestRoom = room;
                }
            }
            startRoom = farthestRoom;
            startRoom.OccupyPoint(startRoom.center);
        }
        else
        {
            startRoom = allRooms[Random.Range(0, allRooms.Count)];
            startRoom.OccupyPoint(startRoom.center);
        }

        //end room
        farthestRoom = null;
        maxDistance = 0;
        foreach (var room in allRooms)
        {
            if (room != startRoom)
            {
                float currentDistance = Vector2.Distance(startRoom.center, room.center);
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    farthestRoom = room;
                }
            }
        }
        endRoom = farthestRoom;
        endRoom.OccupyPoint(endRoom.center);

        //treasure room
        Room rndRoom;
        do
        {
            rndRoom = allRooms[Random.Range(0, allRooms.Count)];
            if (rndRoom != startRoom && rndRoom != endRoom)
                treasureRoom = rndRoom;
        } while (rndRoom == startRoom || rndRoom == endRoom);

        //other rooms
        ordinaryRooms.Clear();
        foreach (var room in allRooms)
        {
            if (room != startRoom && room != endRoom && room != treasureRoom)
            {
                ordinaryRooms.Add(room);
            }
        }
    }



    public void ClearAllObjects()
    {
        tilemapVisualizer?.Clear();

        foreach (var room in ordinaryRooms)
        {
            room?.ClearAvialablePoints();
        }
        startRoom?.ClearAvialablePoints();
        endRoom?.ClearAvialablePoints();
        treasureRoom?.ClearAvialablePoints();

        foreach (var obj in allSpawnedObj)
        {
            if (obj != null)
            {
                DestroyImmediate(obj.gameObject);
            }
        }
        allSpawnedObj = new List<Transform>();
        allSpawnedChest = new List<Transform>();
    }
}
