using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectSpawner))]
[CanEditMultipleObjects]
public class ObjectSpawnerEditor : Editor
{
    private ObjectSpawner _spawner = null;
    void OnEnable()
    {
        _spawner = (ObjectSpawner)target;
        
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Spawn Over Time", GUILayout.Width(160));
        _spawner.SpawnOverTime = EditorGUILayout.Toggle(_spawner.SpawnOverTime);
        GUILayout.EndHorizontal();
        if (_spawner.SpawnOverTime)
        {
            //GUILayout.Space(0);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Seconds Between Spawns", GUILayout.Width(160));
            _spawner.SecondsBetweenSpawns = EditorGUILayout.DelayedFloatField(_spawner.SecondsBetweenSpawns);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Spawn Forever", GUILayout.Width(160));
            _spawner.SpawnForever = EditorGUILayout.Toggle(_spawner.SpawnForever);
            GUILayout.EndHorizontal();

            if (!_spawner.SpawnForever)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Number Of Bursts", GUILayout.Width(160));
                _spawner.NumberOfBursts = EditorGUILayout.DelayedIntField(_spawner.NumberOfBursts);
                GUILayout.EndHorizontal();
            }
        }
    }
}
