using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
