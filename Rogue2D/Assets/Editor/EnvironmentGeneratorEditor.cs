using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractEnvironmentGenerator), true)]
public class EnvironmentGeneratorEditor : Editor
{
    AbstractEnvironmentGenerator generator;


    private void Awake()
    {
        generator = (AbstractEnvironmentGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Environment"))
        {
            generator.GenerateEnvironment();
        }

        if (GUILayout.Button("Open Doors"))
        {
            DungeonEventManager.SendRoomCleared();
        }
        if (GUILayout.Button("Close Doors"))
        {
            DungeonEventManager.SendRoomEntered();
        }
    }
}
