using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractEnemyGenerator), true)]
public class EnemyGeneratorEditor : Editor
{
    AbstractEnemyGenerator generator;


    private void Awake()
    {
        generator = (AbstractEnemyGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Enemy"))
        {
            generator.GenerateEnemy();
        }
    }
}
