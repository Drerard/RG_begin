using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChestSpawner : AbstractSpawnerData
{
    [SerializeField] private bool nearWalls = false;
    [SerializeField] private bool inCenter = false;
    [SerializeField] public bool placeInRoomCenter = false;
    [SerializeField] public bool includeCorner = true;
    public int extraWidth = 0;
    public int extraHeight = 0;
    [Range(0.1f, 100), Space(5)] public float spawnChance = 0.1f;


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
