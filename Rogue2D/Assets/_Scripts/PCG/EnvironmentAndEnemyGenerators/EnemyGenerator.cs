using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGenerator : AbstractEnemyGenerator
{
    [SerializeField] private RateSimpleSpawner spike;
    [SerializeField] private List<EnemiesInRoomSpawner> enemiesInRooms;


    protected override void RunProceduralGeneration()
    {
        GenerateCorridorEnemy();
        GenerateEnemiesInRooms();
    }

    
    private void GenerateCorridorEnemy()
    {
        Vector2Int spawnPos;
        Transform newGameObject;

        foreach (var corridor in dungeon.allCorridors)
        {
            if (spike.spawnChance * 10 >= Random.Range(1, 1001) && corridor.Count >= 5)
            {
                spawnPos = corridor[Random.Range(2, corridor.Count - 2)];
                newGameObject = Instantiate(spike.prefab, (Vector3Int)spawnPos, Quaternion.identity, spike.parent).transform;
                dungeon.allSpawnedObj.Add(newGameObject);
            }
        }
    }

    private void GenerateEnemiesInRooms()
    {
        foreach (var room in dungeon.ordinaryRooms)
        {
            SpawnEnemiesInroom(room, enemiesInRooms);
        }
    }
    private void SpawnEnemiesInroom(Room room, List<EnemiesInRoomSpawner> enemies)
    {
        Vector2Int spawnPos;
        Transform newGameObject;
        List<Vector2Int> availablePoints;

        foreach (var enemy in enemies)
        {
            //add reFindBorderPoints or center with includeCorner flag

            int spawnCount = Random.Range(enemy.minCount, enemy.maxCount + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                availablePoints = room.FindFitPoints(enemy.extraWidth, enemy.extraHeight, enemy.NearWalls, enemy.InCenter);
                if (availablePoints.Count > 0)
                {
                    spawnPos = availablePoints[Random.Range(0, availablePoints.Count)];
                    Vector3 spawnOffsetPos = ((Vector3Int)(spawnPos)) + (Vector3)enemy.offsetSpawnCellPos;
                    newGameObject = Instantiate(enemy.prefab, spawnOffsetPos, Quaternion.identity, enemy.parent).transform;
                    room.AddEnemyToRoom(newGameObject);

                    room.OccupyPoint(spawnPos, enemy.extraWidth, enemy.extraHeight);
                    dungeon.allSpawnedObj.Add(newGameObject);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
