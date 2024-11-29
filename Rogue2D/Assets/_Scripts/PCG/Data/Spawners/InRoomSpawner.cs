using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InRoomSpawner : AbstractSpawnerData
{
    [SerializeField] private bool nearWalls = false;
    [SerializeField] private bool inCenter = false;
    [SerializeField] public bool includeCorner = true;
    public int extraWidth = 0;
    public int extraHeight = 0;
    [Range(0.1f, 100), Space(5)] public float spawnRate = 0.1f;
    
    [HideInInspector] public float distributedSpawnRate = 0.1f;
    [HideInInspector] public int spawnCount = 0;


    public bool NearWalls
    {
        get
        {
            if (nearWalls && inCenter)
                return false;
            return nearWalls;
        }
    }
    public bool InCenter
    {
        get
        {
            if (inCenter && nearWalls)
                return false;
            return inCenter;
        }
    }
}
