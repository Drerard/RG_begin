using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected Dungeon dungeon;
    [SerializeField] protected Vector2Int startPos = Vector2Int.zero;


    public void GenerateDungeon()
    {
        dungeon.ClearAllObjects();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
