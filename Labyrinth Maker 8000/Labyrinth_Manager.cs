using UnityEngine;

/// <summary>
/// Basic wrapper for labyrinth object. Instantiates, passes data and handles setup of labyrinth object and the physical labyrinth.
/// </summary>
public class Labyrinth_Manager : Labyrinth_Manager_Base
{
    [Header("Labyrinth Size Settings")]
    [Tooltip("X and Z scale of the graph. Y is not used.")]
    [SerializeField]
    Vector3 graphScale = new Vector3(100, 0, 100);

    [Tooltip("Size of each cell of the graph. Affects wall length and height in the labyrinth.")]
    [SerializeField]
    float cellSize = 10f;

    [Tooltip("Scales wall height as a multiplier of Cell Size.")]
    [SerializeField]
    float wallYScale = 1f;

    [Tooltip("Scales wall and floor thickness as a multiplier of Cell Size.")]
    [SerializeField]
    [Range(0, 1f)]
    float wallZScale = 0.2f;

    [Header("General Options")]
    [Tooltip("Prevents you from creating graphs of sizes that are known to cause Stack Overflow with a safe margin.\nAlso prevents you from creating labyrinths where the scale is not divisible by Cell Size with a remainder of 0, which causes irregular geometry.")]
    [SerializeField]
    bool safeMode = true;

    [Tooltip("When enabled, generates a labyrinth in the Start() method.")]
    [SerializeField]
    bool generateOnStart = true;

    [Tooltip("When enabled, removes two wall segments from the outer wall on opposing sides.")]
    [SerializeField]
    bool generateExits = true;

    [Tooltip("When enabled, generates the floor in objects of size CellSize by CellSize, instead of one object that is GraphScale by GraphScale.\nNote that this consumes more memory as it instantiates an additional ~(graphScale / cellSize)^2 objects.")]
    [SerializeField]
    bool generateFloorAsTiles = false;

    [Header("Prefabs")]
    [Tooltip("The floor prefab on top of which the labyrinth is built. The floor will be scaled automatically.")]
    [SerializeField]
    GameObject floorPrefab = null;

    [Tooltip("Is the floor object a plane? This is necessary for proper scaling.")]
    [SerializeField]
    bool floorIsPlane = true;

    [Tooltip("The wall prefab which is used for the labyrinth walls. They will be scaled automatically.")]
    [SerializeField]
    GameObject wallPrefab = null;

    [Header("OPTIONAL")]

    [Tooltip("Optional seed to control labyrinth generation and tile replacement.\n\n0 will use the system clock instead per System.Random default.")]
    [SerializeField]
    int seed = 0;

    [Tooltip("Reference a TileReplacer component to replace labyrinth tiles with prefabs from a Tile Codex.\nRequires Generate Floor As Tiles to be enabled.")]
    [SerializeField]
    TileReplacer tileReplacer = null;

    [Tooltip("Extra settings that are for niche cases.")]
    [SerializeField]
    Labyrinth.Extras extras = new Labyrinth.Extras();

    public static Labyrinth Labyrinth { get; set; }

    private void Start()
    {
        if (generateOnStart)
            Generate();
    }

    /// <summary>
    /// Used by the LabyrinthManager_Editor editor extension
    /// </summary>
    public override void Generate()
    {
        if (floorPrefab == null || wallPrefab == null)
        {
            Debug.LogWarning("Floor Prefab or Wall Prefab was null. Returning.");
            return;
        }

        GenerateLabyrinth();
    }

    public void GenerateLabyrinth()
    {
        Labyrinth = new Labyrinth(graphScale, cellSize, 
                                    // Various booleans
                                    safeMode, 
                                    extras.GenerateOuterWall, 
                                    extras.GenerateInnerWall, 
                                    generateFloorAsTiles, 
                                    extras.GenerateRoof, 
                                    floorIsPlane,
                                    extras.ZFightCompensate,
                                    // Prefabs
                                    floorPrefab, 
                                    wallPrefab, 
                                    extras.RoofPrefab,
                                    // Optional Parent
                                    extras.LabyrinthParent);

        Labyrinth_Grapher grapher = new Labyrinth_Grapher();
        Labyrinth.Graph = grapher.GraphFloor(Labyrinth, seed);
        if (Labyrinth.Graph.Nodes.Count < 1)
        {
            Debug.LogError("No vertices were created in the graph. Please check your generation settings. Stopping labyrinth generation.");
            return;
        }
        Labyrinth_Builder builder = new Labyrinth_Builder(Labyrinth, wallYScale, wallZScale);
        Labyrinth.LabyrinthObj = builder.BuildLabyrinth();
        Labyrinth_DFS dfs = new Labyrinth_DFS();
        dfs.DepthFirstSearch(Labyrinth.Graph);
        if (generateExits)
            dfs.GenerateExits(Labyrinth.OuterWall, graphScale, Labyrinth, extras.SingleExit);
        if (tileReplacer != null)
            tileReplacer.ReplaceTiles(Labyrinth, wallZScale, seed);
    }
}
