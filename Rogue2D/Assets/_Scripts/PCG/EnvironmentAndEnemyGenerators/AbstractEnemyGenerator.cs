using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemyGenerator : MonoBehaviour
{
    [SerializeField] protected Dungeon dungeon;
    [SerializeField] protected Vector2Int startPos = Vector2Int.zero;


    public void GenerateEnemy()
    {
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
