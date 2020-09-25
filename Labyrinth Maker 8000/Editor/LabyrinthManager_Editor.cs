using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Labyrinth_Manager_Base), true)]
public class LabyrinthManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        Labyrinth_Manager_Base manager = (Labyrinth_Manager_Base)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Labyrinth"))
        {
            manager.Generate();
        }
    }
}
