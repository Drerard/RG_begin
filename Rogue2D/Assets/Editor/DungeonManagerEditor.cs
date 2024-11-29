using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonManager), true)]
public class DungeonManagerEditor : Editor
{
    DungeonManager dungeonManager;


    private void Awake()
    {
        dungeonManager = (DungeonManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate and start dungeon"))
        {
            dungeonManager.GenerateAndStartDungeon();
        }

        if (GUILayout.Button("Clear all"))
        {
            dungeonManager.ClearAllDungeon();
        }
    }
}
