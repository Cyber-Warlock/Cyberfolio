using System.Collections.Generic;
using UnityEngine;

public class Labyrinth
{
    public readonly Vector3 graphScale;
    public readonly float cellSize;
    public readonly bool safeMode;
    public readonly bool generateOuterWall;
    public readonly bool generateInnerWall;
    public readonly bool generateFloorAsTiles;
    public readonly bool generateRoof;
    public readonly bool zFightCompensate;
    public readonly bool floorIsPlane;

    public readonly GameObject floor;
    public readonly GameObject wall;
    public readonly GameObject roof;

    private GameObject labyrinthObj;

    public Transform parent { get; set; }

    public List<GameObject> OuterWall { get; private set; }

    public List<Graph.Node> ExitNodes { get; private set; } = new List<Graph.Node>(2);

    public List<Filler_Pillar> fillerPillars { get; private set; } = new List<Filler_Pillar>();

    public Graph Graph { get; set; }

    public GameObject LabyrinthObj
    {
        get { return labyrinthObj; }
        // When the labyrinth root object is set, immediately makes it a child of specified parent if there is one
        set
        {
            labyrinthObj = value;
            if (parent != null)
            {
                labyrinthObj.transform.SetParent(parent);
                labyrinthObj.transform.localPosition = Vector3.zero;
            }
        }
    }

    public Labyrinth(Vector3 graphScale, float cellSize, bool safeMode, 
                    bool generateOuterWall, bool generateInnerWall, 
                    bool generateFloorAsTiles, bool generateRoof, 
                    bool floorIsPlane, bool zFightCompensate, 
                    GameObject floorPrefab, 
                    GameObject wallPrefab, 
                    GameObject roofPrefab,
                    Transform parent)
    {
        this.graphScale = graphScale;
        this.cellSize = cellSize;

        this.safeMode = safeMode;
        this.generateOuterWall = generateOuterWall;
        this.generateInnerWall = generateInnerWall;
        this.generateFloorAsTiles = generateFloorAsTiles;
        this.generateRoof = generateRoof;
        this.floorIsPlane = floorIsPlane;
        this.zFightCompensate = zFightCompensate;

        floor = floorPrefab;
        wall = wallPrefab;
        roof = roofPrefab;

        this.parent = parent;

        OuterWall = new List<GameObject>();
    }

    [System.Serializable]
    public class Extras
    {
        [Tooltip("Prevents z-fighting by de-scaling walls and instantiating filler objects. Disabling this will upscale walls to prevent incomplete corners, but local light sources will cause flickering where walls overlap.")]
        [SerializeField]
        bool zFightCompensate = true;

        [Tooltip("When enabled, generates an outer wall around the labyrinth.")]
        [SerializeField]
        bool generateOuterWall = true;

        [Tooltip("When disabled, skips generating the inner walls of the labyrinth. Useful when creating open rooms with a randomized tile layout, using the TileReplacer.")]
        [SerializeField]
        bool generateInnerWall = true;

        [Tooltip("Skips generating a hole in the outer wall on the opposite side of the initially created exit.")]
        [SerializeField]
        bool singleExit = false;

        [Tooltip("Optionally generates a roof.\nNote if the roof prefab is empty, floor or a primitive cube may be used instead.")]
        [SerializeField]
        bool generateRoof = false;

        [SerializeField]
        GameObject roofPrefab = default;

        [Tooltip("The labyrinth will be centered on this transform.")]
        [SerializeField]
        Transform labyrinthParent = null;

        public bool ZFightCompensate
        {
            get { return zFightCompensate; }
        }

        public bool GenerateOuterWall
        {
            get { return generateOuterWall; }
        }

        public bool GenerateInnerWall
        {
            get { return generateInnerWall; }
        }

        public bool SingleExit
        {
            get { return singleExit; }
        }

        public bool GenerateRoof
        {
            get { return generateRoof; }
        }

        public GameObject RoofPrefab
        {
            get { return roofPrefab; }
        }

        public Transform LabyrinthParent
        {
            get { return labyrinthParent; }
        }
    }
}
