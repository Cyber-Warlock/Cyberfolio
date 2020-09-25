using System.Collections.Generic;
using UnityEngine;

public class SO_Labyrinth_Manager : SO_Labyrinth_Manager_Base
{
    [Tooltip("The labyrinth scriptable objects")]
    [SerializeField]
    List<SO_Labyrinth> labyrinths = default;

    [Header("OPTIONAL")]
    [Tooltip("The labyrinth in the matching index will be centered on this transform. Leave it empty if no parent is wanted for the corresponding labyrinth.\nNote: This cannot be deferred to the SO, as Scriptable Objects cannot take scene objects in serialized fields.")]
    [SerializeField]
    List<Transform> labyrinthParent = default;

    [Header("EditMode Generate")]
    [Tooltip("The index of the SO_Labyrinth to generate in EditMode; integer is non-nullable and defaults to 0.")]
    [SerializeField]
    int generateIndex = default;

    /// <summary>
    /// Returns the collection of the SO_Labyrinths. Use it to get one for generation during runtime.
    /// </summary>
    public override List<SO_Labyrinth> Labyrinths
    {
        get { return labyrinths; }
    }

    /// <summary>
    /// For specifying which SO_Labyrinth to generate in EditMode.
    /// Always returns 0 when not in EditMode.
    /// </summary>
    public override int GenerateIndex
    {
        get
        {
            if (Application.isEditor)
                return Mathf.Clamp(generateIndex, 0, labyrinths.Count - 1);
            return 0;
        }
    }

    private void Start()
    {
        if (labyrinths.Count < 1)
            return;

        foreach (SO_Labyrinth l in labyrinths)
        {
            if (l.GenerateOnStart)
            {
                Generate(l);
            }
        }
    }

    public override void Generate(SO_Labyrinth l)
    {
        if (l.FloorPrefab == null || l.WallPrefab == null)
        {
            Debug.LogWarning("Floor Prefab or Wall Prefab was null. Returning.");
            return;
        }

        GenerateLabyrinth(l);
    }

    public void GenerateLabyrinth(SO_Labyrinth l)
    {
        l.Labyrinth = new Labyrinth(l.GraphScale, l.CellSize, 
                                    // Various booleans
                                    l.SafeMode, 
                                    l.Extras.GenerateOuterWall, 
                                    l.Extras.GenerateInnerWall, 
                                    l.GenerateFloorAsCells, 
                                    l.Extras.GenerateRoof, 
                                    l.FloorIsPlane, 
                                    l.Extras.ZFightCompensate,
                                    // Prefabs
                                    l.FloorPrefab, 
                                    l.WallPrefab, 
                                    l.Extras.RoofPrefab,
                                    // Optional Parent
                                    labyrinthParent.Find(p => labyrinthParent.IndexOf(p) == labyrinths.IndexOf(l)));

        Labyrinth_Grapher grapher = new Labyrinth_Grapher();
        l.Labyrinth.Graph = grapher.GraphFloor(l.Labyrinth, l.Seed);
        if (!(l.Labyrinth.Graph.Nodes.Count > 0))
        {
            Debug.LogError("No vertices were created in the graph. Please check your generation settings. Stopping labyrinth generation.");
            return;
        }
        Labyrinth_Builder builder = new Labyrinth_Builder(l.Labyrinth, l.WallYScale, l.WallZScale);
        l.Labyrinth.LabyrinthObj = builder.BuildLabyrinth();
        Labyrinth_DFS dfs = new Labyrinth_DFS();
        dfs.DepthFirstSearch(l.Labyrinth.Graph);
        if (l.GenerateExits)
            dfs.GenerateExits(l.Labyrinth.OuterWall, l.GraphScale, l.Labyrinth, l.Extras.SingleExit);
        if (l.TileReplacer != null)
            l.TileReplacer.ReplaceTiles(l.Labyrinth, l.WallZScale, l.Seed);

        if (!l.SaveGraph)
            l.Labyrinth.Graph = null;
    }
}
