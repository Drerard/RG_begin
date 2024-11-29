using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RateSimpleSpawner : AbstractSpawnerData
{
    [Range(0.1f, 100)] public float spawnChance = 50f;
}
