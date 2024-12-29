using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class EnvironmentGenerator : AbstractEnvironmentGenerator
{
    [SerializeField] private SimpleSpawner nextLevelPassage;
    [Space(10)]
    [SerializeField] private SimpleSpawner door;
    [SerializeField] private SimpleSpawner keyDoor;
    [Space(10)]
    [SerializeField] private List<InRoomSpawner> thingsInRooms;
    [Space(10)]
    [SerializeField] private ChestSpawner chest;
    [SerializeField] private ChestSpawner tresureChest;
    [Space(10)]
    //[SerializeField] private SimpleSpawner decorTorch;
    [SerializeField] private TileBase decorBones;
    [SerializeField] private TileBase decorSkull;


    protected override void RunProceduralGeneration()
    {
        GenerateDoor();
        GenerateChest();
        GeneratePassage();

        GenerateThing();

        GenerateDungeonDecor();
    }


    private void GenerateDoor()
    {
        //ordinaryRooms
        foreach (var room in dungeon.ordinaryRooms)
        {
            SpawnDoor(room, door);
        }

        //startRoom
        SpawnDoor(dungeon.startRoom, door);

        //endRoom
        SpawnDoor(dungeon.endRoom, door);

        //treasureRoom
        SpawnDoor(dungeon.treasureRoom, keyDoor);
    }
    private void SpawnDoor(Room room, SimpleSpawner door)
    {
        Vector2 spawnPos;
        Vector2Int direction;
        Transform newGameObject;

        foreach (var corridor in room.connectedCorridors)
        {
            (spawnPos, direction) = FindNearestCorridorPoint(corridor, room.center);
            newGameObject = Instantiate(door.prefab, spawnPos, Quaternion.identity, door.parent).transform;
            newGameObject.GetComponent<Door>().SetSide(direction);
            dungeon.allSpawnedObj.Add(newGameObject);
        }
    }
    private (Vector2 spawnPos, Vector2Int direction) FindNearestCorridorPoint(List<Vector2Int> corridor, Vector2Int center)
    {
        Vector2Int nearestPos = new Vector2Int();
        Vector2Int direction = new Vector2Int();

        float minDistance = float.MaxValue;
        foreach (var pos in corridor)
        {
            float currentDistance = Vector2.Distance(pos, center);
            if(currentDistance < minDistance)
            {
                minDistance = currentDistance;
                nearestPos = pos;
            }
        }

        if (corridor.Contains(nearestPos + Vector2Int.up) || corridor.Contains(nearestPos + Vector2Int.down))
        {
            direction = Vector2Int.up;
        }
        else
        {
            direction = corridor.Contains(nearestPos + Vector2Int.right) ? Vector2Int.right : Vector2Int.left;
        }

        return (nearestPos, direction);
    }

    private void GenerateChest()
    {
        //ordinaryRooms
        foreach (var room in dungeon.ordinaryRooms)
        {
            SpawnChest(room, chest);
        }

        //treasureRoom
        SpawnChest(dungeon.treasureRoom, tresureChest);

        //put key in random chest
        List<Transform> ordinaryChests = new List<Transform>();
        foreach (var chest in dungeon.allSpawnedChest)
        {
            Chest scriptChest = chest.gameObject.GetComponent<Chest>();
            if (scriptChest != null)
                ordinaryChests.Add(chest);
        }
        if(ordinaryChests.Count > 0)
        {
            ordinaryChests[Random.Range(0, ordinaryChests.Count)].gameObject.GetComponent<Chest>().PutKey();
        }
    }
    private void SpawnChest(Room room, ChestSpawner chest)
    {
        Vector2Int spawnPos;
        Transform newGameObject;
        List<Vector2Int> availablePoints;

        //rolling
        if(chest.spawnChance * 10 >= Random.Range(1, 1001))
        {
            if (chest.placeInRoomCenter && room.availablePoints.Contains(room.center))
            {
                spawnPos = room.center;
                newGameObject = Instantiate(chest.prefab, (Vector3Int)spawnPos, Quaternion.identity, chest.parent).transform;

                room.OccupyPoint(spawnPos, chest.extraWidth, chest.extraHeight);
                dungeon.allSpawnedObj.Add(newGameObject);
                dungeon.allSpawnedChest.Add(newGameObject);
            }
            else
            {
                //add reFindBorderPoints or center with includeCorner flag
                availablePoints = room.FindFitPoints(chest.extraWidth, chest.extraHeight, chest.NearWalls, chest.InCenter);
                if (availablePoints.Count > 0)
                {
                    spawnPos = availablePoints[Random.Range(0, availablePoints.Count)];
                    newGameObject = Instantiate(chest.prefab, (Vector3Int)spawnPos, Quaternion.identity, chest.parent).transform;

                    room.OccupyPoint(spawnPos, chest.extraWidth, chest.extraHeight);
                    dungeon.allSpawnedObj.Add(newGameObject);
                    dungeon.allSpawnedChest.Add(newGameObject);
                }
            }
        }
    }

    private void GenerateThing()
    {
        ProbabilitiesCalc(thingsInRooms);
        
        //ordinaryRooms
        foreach (var room in dungeon.ordinaryRooms)
        {
            SpawnThing(room, thingsInRooms);
        }

        //startRoom
        SpawnThing(dungeon.startRoom, thingsInRooms);

        //endRoom
        SpawnThing(dungeon.endRoom, thingsInRooms);

        //treasureRoom
    }
    private void SpawnThing(Room room, List<InRoomSpawner> things)
    {
        Vector2Int spawnPos;
        Transform newGameObject;
        List<Vector2Int> availablePoints;

        foreach (var thing in things)
        {
            //add reFindBorderPoints or center with includeCorner flag

            thing.spawnCount = (int)((thing.distributedSpawnRate / 100) * room.FitPointsCount(thing.extraWidth, thing.extraHeight, thing.NearWalls, thing.InCenter));
        }
        foreach (var thing in things)
        {
            //add reFindBorderPoints or center with includeCorner flag

            int remainingCount = room.FitPointsCount(thing.extraWidth, thing.extraHeight, thing.NearWalls, thing.InCenter);
            thing.spawnCount = thing.spawnCount <= remainingCount ? thing.spawnCount : remainingCount;
            for (int i = 0; i < thing.spawnCount; i++)
            {
                availablePoints = room.FindFitPoints(thing.extraWidth, thing.extraHeight, thing.NearWalls, thing.InCenter);
                if (availablePoints.Count > 0)
                {
                    spawnPos = availablePoints[Random.Range(0, availablePoints.Count)];
                    newGameObject = Instantiate(thing.prefab, (Vector3Int)spawnPos, Quaternion.identity, thing.parent).transform;

                    room.OccupyPoint(spawnPos, thing.extraWidth, thing.extraHeight);
                    dungeon.allSpawnedObj.Add(newGameObject);
                }
                else
                {
                    break;
                }
            }
        }
    }
    private void ProbabilitiesCalc(List<InRoomSpawner> thingsInRoom)
    {
        float multiRate;
        float multiBorderRate;
        float multiCenterRate;

        float totalRate = 0;
        float totalBorderRate = 0;
        float totalCenterRate = 0;
        foreach (var thingToSpawn in thingsInRoom)
        {
            if (thingToSpawn.NearWalls)
            {
                totalRate += thingToSpawn.spawnCount;
                totalBorderRate += thingToSpawn.spawnCount;
            }
            else if (thingToSpawn.InCenter)
            {
                totalRate += thingToSpawn.spawnCount;
                totalCenterRate += thingToSpawn.spawnCount;
            }
            else
            {
                totalRate += thingToSpawn.spawnCount;
                totalBorderRate += thingToSpawn.spawnCount;
                totalCenterRate += thingToSpawn.spawnCount;
            }
        }

        multiRate = totalRate > 100 ? 100 / totalRate : 1;
        multiBorderRate = totalBorderRate > 100 ? 100 / totalBorderRate : 1;
        multiCenterRate = totalCenterRate > 100 ? 100 / totalCenterRate : 1;

        foreach (var thingToSpawn in thingsInRoom)
        {
            if (thingToSpawn.NearWalls)
            {
                thingToSpawn.distributedSpawnRate = thingToSpawn.spawnRate * multiBorderRate;
            }
            else if (thingToSpawn.InCenter)
            {
                thingToSpawn.distributedSpawnRate = thingToSpawn.spawnRate * multiCenterRate;
            }
            else
            {
                thingToSpawn.distributedSpawnRate = thingToSpawn.spawnRate * multiRate;
            }
        }
    }

    private void GeneratePassage()
    {
        Vector2 spawnPos;
        Transform newGameObject;

        spawnPos = dungeon.endRoom.center;
        newGameObject = Instantiate(nextLevelPassage.prefab, spawnPos, Quaternion.identity, nextLevelPassage.parent).transform;
        dungeon.allSpawnedObj.Add(newGameObject);
    }

    private void GenerateDungeonDecor()
    {
        foreach (var room in dungeon.ordinaryRooms)
        {
            SpawnAllFloorDecor(room, decorBones, 4);
        }

        SpawnAllFloorDecor(dungeon.startRoom, decorBones, 4);

        SpawnAllFloorDecor(dungeon.endRoom, decorBones, 4);

        SpawnAllFloorDecor(dungeon.treasureRoom, decorBones, 10);
        SpawnBorderFloorDecor(dungeon.treasureRoom, decorSkull, 10);
    }
    private void SpawnAllFloorDecor(Room room, TileBase tile, float spawnRate)
    {
        foreach (var point in room.floor)
        {
            if(spawnRate * 10 >= Random.Range(1, 1001))
            {
                dungeon.tilemapVisualizer.PaintDecorTile(point, tile);
            }
        }
    }
    private void SpawnBorderFloorDecor(Room room, TileBase tile, float spawnRate)
    {
        List<Vector2Int> borderFloor = new List<Vector2Int>();
        List<Vector2Int> directions = Direction2D.cardinalDirectionsList;
        foreach (var point in room.floor)
        {
            foreach (var direction in directions)
            {
                if (!room.floor.Contains(point + direction))
                {
                    borderFloor.Add(point);
                    break;
                }
            }
        }

        foreach (var point in borderFloor)
        {
            if (spawnRate * 10 >= Random.Range(1, 1001))
            {
                dungeon.tilemapVisualizer.PaintDecorTile(point, tile);
            }
        }
    }
}
