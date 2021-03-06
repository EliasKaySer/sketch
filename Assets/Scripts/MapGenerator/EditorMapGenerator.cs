using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class EditorMapGenerator : Editor {
    public override void OnInspectorGUI() {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawDefaultInspector()) {
            if (mapGenerator.AutoUpdate) {
                mapGenerator.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate")) {
            mapGenerator.DrawMapInEditor();
        }

        if (GUILayout.Button("Optimizate vlues")) {
            mapGenerator.OptimizateValues();
        }
    }
}