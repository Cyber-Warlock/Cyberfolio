using UnityEngine;

[CreateAssetMenu(fileName = "Labyrinth", menuName = "Labyrinth Maker 8000/Labyrinth")]
public class SO_Labyrinth : ScriptableObject
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
    float wallZScale = 0.2f;

    [Header("General Options")]
    [Tooltip("Prevents you from creating graphs of sizes that are known to cause Stack Overflow with a safe margin.\nAlso prevents you from creating labyrinths where the scale is not divisible by Cell Size, which causes irregular geometry.")]
    [SerializeField]
    bool safeMode = true;

    [Tooltip("When enabled, generates a labyrinth when the scene loads.")]
    [SerializeField]
    bool generateOnStart = true;

    [Tooltip("When enabled, generates an outer wall around the labyrinth.")]
    [SerializeField]
    bool generateOuterWall = true;

    [Tooltip("When enabled, removes two wall segments from the outer wall on opposing sides.")]
    [SerializeField]
    bool generateExits = true;

    [Tooltip("When enabled, generates the floor in objects of size CellSize by CellSize, instead of one object that is GraphScale by GraphScale.\nNote that this consumes more memory.")]
    [SerializeField]
    bool generateFloorAsCells = false;

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

    [Tooltip("Optionally saves the generated graph or lets the garbage collector handle it after generation.")]
    [SerializeField]
    bool saveGraph = false;

    [Tooltip("Optional name for the labyrinth SO. Can make it easier to locate in the manager.")]
    [SerializeField]
    string labyrinthName = "";
    
    [Tooltip("Reference a TileReplacer component to replace labyrinth tiles with prefabs from a Tile Codex.\nRequires Generate Floor As Tiles to be enabled.")]
    [SerializeField]
    TileReplacer tileReplacer = null;

    [Tooltip("Extra settings that are for niche cases.")]
    [SerializeField]
    Labyrinth.Extras extras = new Labyrinth.Extras();

    public Labyrinth Labyrinth { get; set; }

    public Vector3 GraphScale
    {
        get { return graphScale; }
    }

    public float CellSize
    {
        get { return cellSize; }
    }

    public float WallYScale
    {
        get { return wallYScale; }
    }

    public float WallZScale
    {
        get { return wallZScale; }
    }

    public bool SafeMode
    {
        get { return safeMode; }
    }

    public bool GenerateOnStart
    {
        get { return generateOnStart; }
    }

    public bool GenerateOuterWall
    {
        get { return generateOuterWall; }
    }

    public bool GenerateExits
    {
        get { return generateExits; }
    }

    public bool GenerateFloorAsCells
    {
        get { return generateFloorAsCells; }
    }

    public bool FloorIsPlane
    {
        get { return floorIsPlane; }
    }

    public GameObject FloorPrefab
    {
        get { return floorPrefab; }
    }

    public GameObject WallPrefab
    {
        get { return wallPrefab; }
    }

    public int Seed
    {
        get { return seed; }
    }

    public bool SaveGraph
    {
        get { return saveGraph; }
    }

    public string LabyrinthName
    {
        get { return labyrinthName; }
    }

    public TileReplacer TileReplacer
    {
        get { return tileReplacer; }
    }

    public Labyrinth.Extras Extras
    {
        get { return extras; }
    }
}
