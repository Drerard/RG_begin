using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLogicDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected Dungeon dungeon;
    [SerializeField] protected Vector2Int startPos = Vector2Int.zero;


    public void GenerateLogic()
    {
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
