using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_Labyrinth_Manager_Base), true)]
public class SO_Labyrinth_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SO_Labyrinth_Manager_Base manager = (SO_Labyrinth_Manager_Base)target;
        if (GUILayout.Button("Generate Labyrinth"))
        {
            manager.Generate(manager.Labyrinths[manager.GenerateIndex]);
        }
    }
}
