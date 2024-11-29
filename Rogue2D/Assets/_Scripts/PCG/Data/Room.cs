using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room
{
    public BoundsInt size;
    public Vector2Int center;
    public HashSet<Vector2Int> floor;
    public List<List<Vector2Int>> connectedCorridors = new List<List<Vector2Int>>();
    
    public List<Vector2Int> availablePoints;
    private List<Vector2Int> borderAvailablePoints;
    private List<Vector2Int> centerAvailablePoints;

    private Transform playerPos;
    private List<Transform> roomEnemies = new List<Transform>();
    private int remaindEnemies = 0;


    public Room(BoundsInt bounds, HashSet<Vector2Int> floorPos)
    {
        size = bounds;
        floor = floorPos;
        availablePoints = new List<Vector2Int>(floorPos);
        borderAvailablePoints = FindBorderPoints(availablePoints);
        centerAvailablePoints = FindCenterPoints(availablePoints);

        center = (Vector2Int) Vector3Int.RoundToInt(bounds.center);
    }


    public void AddEnemyToRoom(Transform enemy)
    {
        remaindEnemies++;
        roomEnemies.Add(enemy);
        enemy.GetComponent<DungeonMonster>().SetOwnRoom(this);
    }

    public void SendEnemyKilled()
    {
        remaindEnemies--;
        if(remaindEnemies <= 0)
        {
            DungeonEventManager.SendRoomCleared();
        }
    }

    public void SendPlayerEntered(Transform player)
    {
        DungeonEventManager.SendRoomEntered();
        playerPos = player;
        ActivateEnemies();
    }
    private void ActivateEnemies()
    {
        foreach (var enemy in roomEnemies)
        {
            if (enemy != null)
            {
                enemy?.GetComponent<DungeonMonster>().Activate();
            }
        }
    }

    public int FitPointsCount(int extraX, int extraY, bool nearWall, bool inCenter)
    {
        List<Vector2Int> fitPoints = new List<Vector2Int>();
        List<Vector2Int> maskPoints;
        List<Vector2Int> tempAvailablePoints;
        bool fit;

        if (nearWall)
            maskPoints = new List<Vector2Int>(borderAvailablePoints);
        else if (inCenter)
            maskPoints = new List<Vector2Int>(centerAvailablePoints);
        else
            maskPoints = new List<Vector2Int>(availablePoints);

        tempAvailablePoints = new List<Vector2Int>(maskPoints);

        foreach (var point in maskPoints)
        {
            fit = true;
            for (int x = -extraX; x <= extraX; x++)
            {
                for (int y = -extraY; y <= extraY; y++)
                {
                    if (!tempAvailablePoints.Contains(point + new Vector2Int(x, y)))
                        fit = false;
                }
            }
            if (fit)
            {
                fitPoints.Add(point);
                TempOccupyPoint(tempAvailablePoints, point, extraX, extraY);
            }
        }
        return fitPoints.Count;
    }
    public List<Vector2Int> FindFitPoints(int extraX, int extraY, bool nearWall, bool inCenter)
    {
        List<Vector2Int> fitPoints = new List<Vector2Int>();
        List<Vector2Int> maskAvailablePoints;
        bool fit;

        if (nearWall)
            maskAvailablePoints = borderAvailablePoints;
        else if (inCenter)
            maskAvailablePoints = centerAvailablePoints;
        else
            maskAvailablePoints = availablePoints;

        foreach (var point in maskAvailablePoints)
        {
            fit = true;
            for (int x = -extraX; x <= extraX; x++)
            {
                for (int y = -extraY; y <= extraY; y++)
                {
                    if (!maskAvailablePoints.Contains(point + new Vector2Int(x, y)))
                        fit = false;
                }
            }
            if (fit)
            {
                fitPoints.Add(point);
            }
        }
        return fitPoints;
    }

    public void OccupyPoint(Vector2Int pos)
    {
        availablePoints.Remove(pos);
        borderAvailablePoints.Remove(pos);
        centerAvailablePoints.Remove(pos);
    }
    public void OccupyPoint(Vector2Int pos, int extraX, int extraY)
    {
        for (int x = -extraX; x <= extraX; x++)
        {
            for (int y = -extraY; y <= extraY; y++)
            {
                availablePoints.Remove(pos + new Vector2Int(x, y));
                borderAvailablePoints.Remove(pos + new Vector2Int(x, y));
                centerAvailablePoints.Remove(pos + new Vector2Int(x, y));
            }
        }
    }
    private void TempOccupyPoint(List<Vector2Int> tempAvailablePoints, Vector2Int pos, int extraX, int extraY)
    {
        for (int x = -extraX; x <= extraX; x++)
        {
            for (int y = -extraY; y <= extraY; y++)
            {
                tempAvailablePoints.Remove(pos + new Vector2Int(x, y));
            }
        }
    }


    private List<Vector2Int> FindBorderPoints(List<Vector2Int> allPoints)
    {
        List<Vector2Int> borderPoints = new List<Vector2Int>();
        //List<Vector2Int> directions = Direction2D.aroundDirectionsList;
        List<Vector2Int> directions = Direction2D.cardinalDirectionsList;

        foreach (var point in allPoints)
        {
            foreach (var direction in directions)
            {
                if (!allPoints.Contains(point + direction) && !floor.Contains(point + direction))
                {
                    borderPoints.Add(point);
                    break;
                }
            }
        }

        return borderPoints;
    }
    private List<Vector2Int> FindCenterPoints(List<Vector2Int> allPoints)
    {
        List<Vector2Int> centerPoints = new List<Vector2Int>();
        //List<Vector2Int> directions = Direction2D.aroundDirectionsList;
        List<Vector2Int> directions = Direction2D.cardinalDirectionsList;
        bool inCenter;

        foreach (var point in allPoints)
        {
            inCenter = true;
            foreach (var direction in directions)
            {
                if (!allPoints.Contains(point + direction))
                {
                    inCenter = false;
                    break;
                }
            }
            if (inCenter)
            {
                centerPoints.Add(point);
            }
        }
        return centerPoints;
    }


    public void ClearAvialablePoints()
    {
        availablePoints = new List<Vector2Int>(floor);
        foreach (var currentCorridor in connectedCorridors)
        {
            foreach (var pos in currentCorridor)
            {
                OccupyPoint(pos, 1, 1);
            }
        }

        borderAvailablePoints = FindBorderPoints(availablePoints);
        centerAvailablePoints = FindCenterPoints(availablePoints);
    }
}
