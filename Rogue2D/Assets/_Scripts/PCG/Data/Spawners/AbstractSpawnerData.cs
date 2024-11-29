using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class AbstractSpawnerData
{
    public GameObject prefab;
    public Transform parent;
}
