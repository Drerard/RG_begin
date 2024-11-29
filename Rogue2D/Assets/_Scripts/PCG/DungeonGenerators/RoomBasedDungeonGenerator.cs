using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomBasedDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [Space(5)]
    [SerializeField] private int minRoomWidth = 20;
    [SerializeField] private int minRoomHeight = 20;

    [SerializeField] private int dungeonWidth = 200;
    [SerializeField] private int dungeonHeight = 200;

    [SerializeField, Range(0, 10)] private int offset = 3;

    [SerializeField] private bool randomWalkRooms = false;
    [SerializeField] private bool placeStartRoomInCorner = true;

    [SerializeField, Space(10)] private bool showRoomsCenter = false;


    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        BoundsInt initialSpace = new BoundsInt((Vector3Int)startPos, new Vector3Int(dungeonWidth, dungeonHeight, 0));
        List<BoundsInt> roomsBounds = ProceduralGenerationAlgoritms.BinarySpacePartitioning(initialSpace, minRoomWidth, minRoomHeight);

        List<HashSet<Vector2Int>> roomsFloor;
        if (randomWalkRooms)
        {
            roomsFloor = CreateRoomsRndWalk(roomsBounds);
        }
        else
        {
            roomsFloor = CreateSimpleRooms(roomsBounds);
        }

        List<Room> allRooms = new List<Room>();
        for (int i = 0; i < roomsBounds.Count; i++)
        {
            allRooms.Add(new Room(roomsBounds[i], roomsFloor[i]));
        }
        dungeon.AssignKeyRooms(allRooms, initialSpace, placeStartRoomInCorner);

        List<(Room, Room)> mst = ProceduralGenerationAlgoritms.MstGeneration(dungeon.startRoom, dungeon.ordinaryRooms);
        LinkRoomToMst(mst, dungeon.ordinaryRooms, dungeon.endRoom);
        LinkRoomToMst(mst, dungeon.ordinaryRooms, dungeon.treasureRoom);

        List<List<Vector2Int>> corridors = CreateCorridors(mst);//and addet attached corridors to rooms
        dungeon.allCorridors = corridors;

        VisualizeFloorAndWall(allRooms, corridors);
    }


    private List<HashSet<Vector2Int>> CreateRoomsRndWalk(List<BoundsInt> roomsBounds)
    {
        List<HashSet<Vector2Int>> roomsFloor = new List<HashSet<Vector2Int>>();

        for (int i = 0; i < roomsBounds.Count; i++)
        {
            Vector2Int roomCenter = (Vector2Int) Vector3Int.RoundToInt(roomsBounds[i].center);
            HashSet<Vector2Int> roomFloor = RunRandomWalk(roomCenter, roomsBounds[i], offset);
            
            foreach (var pos in roomFloor)
            {
                bool fitX = (roomsBounds[i].xMin + offset <= pos.x) && (pos.x <= roomsBounds[i].xMax - offset);
                bool fitY = (roomsBounds[i].yMin + offset <= pos.y) && (pos.y <= roomsBounds[i].yMax - offset);
                if (!(fitX && fitY))
                {
                    roomFloor.Remove(pos);
                }
            }
            roomsFloor.Add(roomFloor);
        }
        return roomsFloor;
    }
    private List<HashSet<Vector2Int>> CreateSimpleRooms(List<BoundsInt> roomsBounds)
    {
        List<HashSet<Vector2Int>> roomsFloor = new List<HashSet<Vector2Int>>();

        for (int i = 0; i < roomsBounds.Count; i++)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();

            for (int col = offset; col < roomsBounds[i].size.x - offset; col++)
            {
                for (int row = offset; row < roomsBounds[i].size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)roomsBounds[i].min + new Vector2Int(col, row);
                    roomFloor.Add(pos);
                }
            }
            roomsFloor.Add(roomFloor);
        }
        return roomsFloor;
    }

    private void LinkRoomToMst(List<(Room, Room)> mst, List<Room> ordinaryRooms, Room linkedRoom)
    {
        float minDistance = float.MaxValue;
        Room nearestRoom = null;

        foreach (var room in ordinaryRooms)
        {
            float currentDistance = Vector2.Distance(linkedRoom.center, room.center);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                nearestRoom = room;
            }
        }

        if(nearestRoom != null)
        {
            mst.Add((nearestRoom, linkedRoom));
        }
    }

    private List<List<Vector2Int>> CreateCorridors(List<(Room, Room)> mst)
    {
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        List<Vector2Int> currentCorridor;
        foreach (var edge in mst)
        {
            (Vector2Int startPos, Vector2Int destination) = FindNearestPoints(edge.Item1.floor, edge.Item2.floor);

            List<Vector2Int> spaceToDoor;
            Vector2Int coordDirection;

            (spaceToDoor, coordDirection, startPos, destination) = AddSpaceToDoor(startPos, destination, edge.Item1, edge.Item2);
            currentCorridor = ProceduralGenerationAlgoritms.BresenhamLineCorridor(startPos, destination);
            currentCorridor.AddRange(spaceToDoor);
            currentCorridor = SortByDirection(currentCorridor, coordDirection);

            edge.Item1.connectedCorridors.Add(currentCorridor);
            edge.Item2.connectedCorridors.Add(currentCorridor);
            foreach (var pos in currentCorridor)
            {
                edge.Item1.OccupyPoint(pos, 1, 1);
                edge.Item2.OccupyPoint(pos, 1, 1);
            }

            corridors.Add(currentCorridor);
        }
        return corridors;
    }
    private (Vector2Int, Vector2Int) FindNearestPoints(HashSet<Vector2Int> collectionPoints1, HashSet<Vector2Int> collectionPoints2)
    {
        KdTree kdTreePoints1 = new KdTree(FindBorderPoints(collectionPoints1));
        List<Vector2Int> borderPoints2 = FindBorderPoints(collectionPoints2);

        Vector2Int startPoint = Vector2Int.zero;
        Vector2Int destination = Vector2Int.zero;

        float minDistance = float.MaxValue;
        foreach (var point2 in borderPoints2)
        {
            Vector2Int nearestStartPoint = kdTreePoints1.FindNearestNeighbor(point2);
            float currentDistance = Vector2.Distance(nearestStartPoint, point2);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                startPoint = nearestStartPoint;
                destination = point2;
            }
        }
        return (startPoint, destination);
    }
    private List<Vector2Int> FindBorderPoints(HashSet<Vector2Int> collectionPoints)
    {
        List<Vector2Int> borderPoints = new List<Vector2Int>();
        List<Vector2Int> directions = Direction2D.cardinalDirectionsList;

        foreach (var pos in collectionPoints)
        {
            foreach (var direction in directions)
            {
                if (!collectionPoints.Contains(pos + direction))
                {
                    borderPoints.Add(pos);
                    break;
                }
            }
        }
        return borderPoints;
    }
    private (List<Vector2Int> spaceToDoor, Vector2Int direction, Vector2Int startPos, Vector2Int destination) AddSpaceToDoor(Vector2Int startPos, Vector2Int destination, Room room1, Room room2)
    {
        List<Vector2Int> spaceToDoor = new List<Vector2Int>();
        Vector2Int newStartPos = startPos;
        Vector2Int newDestination = destination;
        Vector2Int center1 = room1.center;
        Vector2Int center2 = room2.center;

        Vector2Int corridorDirection;
        Vector2Int neighborFloor;
        Vector2Int coordinateDirection = new Vector2Int();
        coordinateDirection.x = center2.x > center1.x ? 1 : -1;
        coordinateDirection.y = center2.y > center1.y ? 1 : -1;

        if (Math.Abs(center2.x - center1.x) > Math.Abs(center2.y - center1.y))
        {
            corridorDirection = center2.x > center1.x ? Vector2Int.right : Vector2Int.left;
            neighborFloor = Vector2Int.up;
        }
        else
        {
            corridorDirection = center2.y > center1.y ? Vector2Int.up : Vector2Int.down;
            neighborFloor = Vector2Int.right;
        }

        for (int i = 0; i < 2; i++)
        {
            newStartPos += corridorDirection;
            spaceToDoor.Add(newStartPos);
            room1.floor.Remove(neighborFloor);
            room1.floor.Remove(neighborFloor * -1);

            newDestination -= corridorDirection;
            spaceToDoor.Add(newDestination);
            room2.floor.Remove(neighborFloor);
            room2.floor.Remove(neighborFloor * -1);
        }

        return (spaceToDoor, coordinateDirection, newStartPos, newDestination);
    }
    private List<Vector2Int> SortByDirection(List<Vector2Int> corridor, Vector2Int coordDirection)
    {
        List<Vector2Int> sortedCorridor;

        if (coordDirection.x > 0)
        {
            if (coordDirection.y > 0)
            {
                sortedCorridor = corridor.OrderBy(c => c.x).ThenBy(c => c.y).ToList();
            }
            else
            {
                sortedCorridor = corridor.OrderBy(c => c.x).ThenByDescending(c => c.y).ToList();
            }
        }
        else
        {
            if (coordDirection.y > 0)
            {
                sortedCorridor = corridor.OrderByDescending(c => c.x).ThenBy(c => c.y).ToList();
            }
            else
            {
                sortedCorridor = corridor.OrderByDescending(c => c.x).ThenByDescending(c => c.y).ToList();
            }
        }

        return sortedCorridor;
    }

    private void VisualizeFloorAndWall(List<Room> allRooms, List<List<Vector2Int>> corridors)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var room in allRooms)
        {
            floor.UnionWith(room.floor);
        }
        foreach (var corridor in corridors)
        {
            floor.UnionWith(corridor);
        }

        dungeon.tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, dungeon.tilemapVisualizer);

        if (showRoomsCenter)
        {
            foreach (var room in allRooms)
            {
                //dungeon.tilemapVisualizer.PaintRoomCenterTile(room.center);
            }

            dungeon.tilemapVisualizer.PaintRoomCenterTile(dungeon.startRoom.center);
            dungeon.tilemapVisualizer.PaintRoomCenterTile(dungeon.endRoom.center);
            dungeon.tilemapVisualizer.PaintRoomCenterTile(dungeon.treasureRoom.center);

            foreach (var corridor in corridors)
            {
                foreach (var pos in corridor)
                {
                    dungeon.tilemapVisualizer.PaintCorridorTile(pos);
                }
            }
        }
    }
}
