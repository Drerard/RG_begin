using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemiesInRoomSpawner : AbstractSpawnerData
{
    [SerializeField] private bool nearWalls = false;
    [SerializeField] private bool inCenter = false;
    [SerializeField] public bool includeCorner = true;
    public int minCount = 0;
    public int maxCount = 0;
    [Space(5)]
    public Vector2 offsetSpawnCellPos = Vector2.zero; 
    public int extraWidth = 0;
    public int extraHeight = 0;
    

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
