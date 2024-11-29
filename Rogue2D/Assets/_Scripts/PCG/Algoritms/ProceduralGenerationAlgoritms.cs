using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGenerationAlgoritms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPos, int walkLenght)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPos);
        Vector2Int previousPos = startPos;

        for (int i = 0; i < walkLenght; i++)
        {
            Vector2Int newPos = previousPos + Direction2D.GetRndCardinalDirection();
            path.Add(newPos);
            previousPos = newPos;
        }
        return path;
    }
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPos, int walkLenght, BoundsInt roomParam, int offset)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPos);
        Vector2Int previousPos = startPos;

        for (int i = 0; i < walkLenght; i++)
        {
            Vector2Int newPos = previousPos + Direction2D.GetRndCardinalDirection();

            bool fitX = (roomParam.xMin + offset <= newPos.x) && (newPos.x <= roomParam.xMax - offset);
            bool fitY = (roomParam.yMin + offset <= newPos.y) && (newPos.y <= roomParam.yMax - offset);
            if (fitX && fitY)
            {
                path.Add(newPos);
                previousPos = newPos;
            }
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPos, int corridorLenght)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRndCardinalDirection();
        var currentPos = startPos;
        corridor.Add(currentPos);

        for(int i= 0; i< corridorLenght; i++)
        {
            currentPos += direction;
            corridor.Add(currentPos);
        }
        return corridor;
    }

    public static List<Vector2Int> BresenhamLineCorridor(Vector2Int startPos, Vector2Int destination)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();

        int x1 = startPos.x;
        int y1 = startPos.y;
        int x2 = destination.x;
        int y2 = destination.y;
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = x2 >= x1 ? 1 : -1;
        int sy = y2 >= y1 ? 1 : -1;
        Vector2Int lastPos = Vector2Int.zero;

        if (dx == 0 && dy == 0)
            return corridor;

        float step = 1.0f / Math.Max(dx, dy);
        int maxD = Math.Max(dx, dy);
        float j = 0;
        for (float i = 0; i <= maxD; i++)
        {
            int xChange = (int)(dx * j + 0.5f) - (int)(dx * (j - step) + 0.5f);
            int yChange = (int)(dy * j + 0.5f) - (int)(dy * (j - step) + 0.5f);
            if (xChange != 0 && yChange != 0 && j != 0)
            {
                int prevX = x1 + (int)(dx * (j - step) + 0.5f) * sx;
                int prevY = y1 + (int)(dy * j + 0.5f) * sy;
                corridor.Add(new Vector2Int(prevX, prevY));
            }

            int x = x1 + (int)(dx * j + 0.5f) * sx;
            int y = y1 + (int)(dy * j + 0.5f) * sy;
            corridor.Add(new Vector2Int(x, y));

            lastPos = new Vector2Int(x, y);

            j += step;
        }

        corridor.Remove(lastPos);
        corridor.RemoveAt(0);
        return corridor;
    }

    public static List<(Room, Room)> MstGeneration(Room startRoom, List<Room> ordinaryRooms)
    {
        List<(Room, Room)> mst = new List<(Room, Room)>();

        List<(Room, Room, float)> edges = new System.Collections.Generic.List<(Room, Room, float)>();
        HashSet<Room> unvisitedRooms = new HashSet<Room>(ordinaryRooms);
        HashSet<Room> visitedRooms = new HashSet<Room>();

        foreach (var room in unvisitedRooms)
        {
            float distance = Vector2.Distance(startRoom.center, room.center);
            edges.Add((startRoom, room, distance));
        }
        visitedRooms.Add(startRoom);

        while(visitedRooms.Count < ordinaryRooms.Count + 1)
        {
            float minDistance = float.MaxValue;
            (Room, Room, float) shortestEdge = new(null, null, 0);

            foreach (var edge in edges)
            {
                float distance = edge.Item3;
                if (unvisitedRooms.Contains(edge.Item2))
                {
                    if(distance < minDistance)
                    {
                        minDistance = distance;
                        shortestEdge = edge;
                    }
                }
            }

            mst.Add((shortestEdge.Item1, shortestEdge.Item2));
            unvisitedRooms.Remove(shortestEdge.Item2);
            visitedRooms.Add(shortestEdge.Item2);

            edges.Remove(shortestEdge);
            foreach (var room in unvisitedRooms)
            {
                float distance = Vector2.Distance(shortestEdge.Item2.center, room.center);
                edges.Add((shortestEdge.Item2, room, distance));
            }
        }
        return mst;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();

            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                        SplitHorizontally(minHeight, roomsQueue, room);
                    else if (room.size.x >= minWidth * 2)
                        SplitVertically(minWidth, roomsQueue, room);
                    else if (room.size.y >= minHeight && room.size.x >= minWidth)
                        roomsList.Add(room);
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                        SplitVertically(minWidth, roomsQueue, room);
                    else if (room.size.y >= minHeight * 2)
                        SplitHorizontally(minHeight, roomsQueue, room);
                    else if (room.size.y >= minHeight && room.size.x >= minWidth)
                        roomsList.Add(room);
                }
            }
        }
        return roomsList;
    }
    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        int xSplit = Random.Range(minWidth, room.size.x - minWidth);//minWidth, room.size.x - minWidth
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        int ySplit = Random.Range(minHeight, room.size.y - minHeight);//minHeight, room.size.y - minHeight
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}


public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>()
    {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(0,-1), //DOWN
        new Vector2Int(-1,0) //LEFT
    };
    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>()
    {
        new Vector2Int(1,1), //UP RIGHT
        new Vector2Int(1,-1), //RIGHT DOWN
        new Vector2Int(-1,-1), //DOWN LEFT
        new Vector2Int(-1,1) //LEFT UP
    };
    public static List<Vector2Int> aroundDirectionsList = new List<Vector2Int>()
    {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,1), //UP RIGHT
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(1,-1), //RIGHT DOWN
        new Vector2Int(0,-1), //DOWN
        new Vector2Int(-1,-1), //DOWN LEFT
        new Vector2Int(-1,0), //LEFT
        new Vector2Int(-1,1) //LEFT UP
    };

    public static Vector2Int GetRndCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
    public static Vector2Int GetRndDiagonalDirection()
    {
        return diagonalDirectionsList[Random.Range(0, diagonalDirectionsList.Count)];
    }
}
