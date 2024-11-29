using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootSpawner 
{
    public GameObject prefab;
    [Range(0.1f, 100)] public float spawnChance = 50f;
    public int minCount = 0;
    public int maxCount = 0;
    public Vector2 rndOffsetSpawnPos = Vector2.zero;
}
