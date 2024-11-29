using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawner.asset", menuName = "Spawners/Spawner", order = 51)]
public class ScriptableSpawnerData : ScriptableObject
{
    public GameObject prefabToSpawn;
    [SerializeField] private bool nearWalls = false;
    [SerializeField] private bool inCenter = false;
    public int extraWidth = 0;
    public int extraHeight = 0;


    public bool NearWalls
    {
        get
        {
            if(nearWalls && inCenter)
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
