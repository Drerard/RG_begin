using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractLogicDungeonGenerator), true)]
public class LogicDungeonGeneratorEditor : Editor
{
    AbstractLogicDungeonGenerator generator;


    private void Awake()
    {
        generator = (AbstractLogicDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create Logic"))
        {
            generator.GenerateLogic();
        }
    }
}
