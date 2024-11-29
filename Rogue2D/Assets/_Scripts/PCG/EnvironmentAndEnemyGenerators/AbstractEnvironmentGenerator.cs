using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnvironmentGenerator : MonoBehaviour
{
    [SerializeField] protected Dungeon dungeon;
    [SerializeField] protected Vector2Int startPos = Vector2Int.zero;


    public void GenerateEnvironment()
    {
        //dungeon.ClearAllObjects();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
