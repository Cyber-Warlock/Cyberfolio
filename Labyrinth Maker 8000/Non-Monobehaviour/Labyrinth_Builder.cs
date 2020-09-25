using System.Collections.Generic;
using UnityEngine;

public class Labyrinth_Builder
{
    const float PLANE_DESCALE = 10f;

    readonly Labyrinth l;
    readonly float wallYScale;
    readonly float wallThickness;

    GameObject labyrinth;
    GameObject wallObj;

    public Labyrinth_Builder(Labyrinth l, float wallYScale, float wallZScale)
    {
        this.l = l;
        this.wallYScale = wallYScale;
        wallThickness = l.cellSize * wallZScale;
    }

    public GameObject BuildLabyrinth()
    {
        // Create the floor upon which to build the labyrinth walls
        // If GenerateFloorAsCells is true, then each node will have its own floor tile, parented to an anchor.
        // Otherwise the floor will be one object which functions as the anchor.
        if (l.generateFloorAsTiles)
        {
            // Scale prefab once
            l.floor.transform.localScale = l.floorIsPlane ? new Vector3(l.cellSize / PLANE_DESCALE, l.cellSize / PLANE_DESCALE, l.cellSize / PLANE_DESCALE) :
                                                                new Vector3(l.cellSize, wallThickness, l.cellSize);

            // If there is a roof, scale it once
            if (l.roof != null)
                l.roof.transform.localScale = new Vector3(l.cellSize + wallThickness, wallThickness, l.cellSize + wallThickness);

            labyrinth = new GameObject("Labyrinth_Anchor");
            foreach (Graph.Node node in l.Graph.Nodes)
            {
                GameObject floor = Object.Instantiate(l.floor, node.CellCenter, Quaternion.identity, labyrinth.transform);
                node.Floor = floor;

                // If a roof should be generated, make the tiles concurrently, reusing floor if there is no roof
                if (l.generateRoof)
                    GenerateRoof(l.roof == null ? floor : l.roof, node.CellCenter);
            }
        }
        else
        {
            // Scale prefab so scale is carried to roof
            l.floor.transform.localScale = !l.floorIsPlane ? new Vector3(l.graphScale.x, wallThickness, l.graphScale.z) :
                 new Vector3(l.graphScale.x / PLANE_DESCALE, l.cellSize / PLANE_DESCALE, l.graphScale.z / PLANE_DESCALE);

            labyrinth = Object.Instantiate(l.floor);

            if (l.generateRoof)
                GenerateRoof(labyrinth, labyrinth.transform.position);
        }

        // Leftover from refactoring
        float halfCell = l.cellSize / 2;
        Vector3 halfScale = l.graphScale / 2;

        // Should inner walls be generated?
        if (l.generateInnerWall)
            GenerateInnerWalls(halfCell);

        // Should other walls be generated?
        if (l.generateOuterWall)
            GenerateOuterWalls(halfCell, halfScale);

        return labyrinth;
    }

    void GenerateInnerWalls(float halfCell)
    {
        // Rescale the prefab to specifications
        l.wall.transform.localScale = new Vector3(l.cellSize + (l.zFightCompensate ? -wallThickness : wallThickness), l.cellSize * wallYScale, wallThickness);

        Vector3 pos, rot;

        foreach (Graph.Node node in l.Graph.Nodes)
        {
            foreach (Graph.Edge edge in node.Edges)
            {
                if (edge.Wall == null)
                {
                    // If the x value of the neighbours subtracted equals Cell Size, the wall must have a rotation of 90 in y to be perpendicular
                    // Otherwise it must be 0
                    if (edge.Node_s.CellCenter.x - edge.Node_f.CellCenter.x == l.cellSize)
                    {
                        pos = new Vector3(edge.Node_s.CellCenter.x - halfCell, halfCell, edge.Node_s.CellCenter.z);
                        rot = new Vector3(0, 90, 0);
                    }
                    else
                    {
                        pos = new Vector3(edge.Node_s.CellCenter.x, halfCell, edge.Node_s.CellCenter.z - halfCell);
                        rot = Vector3.zero;
                    }
                    // Instantiate the object with the needed rotation
                    wallObj = Object.Instantiate(l.wall, pos, Quaternion.Euler(rot));
                    // Make it a child of the labyrinth anchor for order's sake
                    wallObj.transform.SetParent(labyrinth.transform);
                    // Set the wall on the edge, important for the DFS
                    edge.Wall = wallObj;
                }
            }
        }

        if (l.zFightCompensate)
            GeneratePillarFill();
    }

    /// <summary>
    /// Generate a roof parented to the labyrinth. Object will be flipped 180 degrees around the X-axis.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="center">Y-offset is added automatically</param>
    void GenerateRoof(GameObject template, Vector3 center)
    {
        // Instantiate the pre-scaled prefab, add the roof Y-offset to its position, flip it 180 degrees and parent it
        Object.Instantiate(template, center + new Vector3(0, l.cellSize * wallYScale, 0), Quaternion.Euler(180f, 0, 0), labyrinth.transform);
    }

    void GeneratePillarFill()
    {
        int xJunctions = (int)(l.graphScale.x - l.cellSize * 2) / 2;
        int zJunctions = (int)(l.graphScale.z - l.cellSize * 2) / 2;

        for (int x = -xJunctions; x <= xJunctions; x += (int)l.cellSize)
        {
            for (int z = -zJunctions; z <= zJunctions; z += (int)l.cellSize)
            {
                GameObject pillar = Object.Instantiate(l.wall, labyrinth.transform);

                pillar.transform.localPosition = new Vector3(x, l.cellSize / 2, z);
                pillar.transform.localScale = new Vector3(wallThickness, l.cellSize * wallYScale, wallThickness);
                pillar.transform.rotation = Quaternion.identity;

                AddFillerPillar(pillar);
            }
        }

        void AddFillerPillar(GameObject pillarObj)
        {
            Filler_Pillar pillar = new Filler_Pillar(pillarObj);

            Graph.Node root = l.Graph.Nodes.Find(n => n.CellCenter == new Vector3(pillarObj.transform.position.x - l.cellSize / 2,
                                                                                  pillarObj.transform.position.y - l.cellSize / 2,
                                                                                  pillarObj.transform.position.z - l.cellSize / 2));

            pillar.PopulateEdges(l.Graph.GetRectEdges(root, 2, 2, l.cellSize));

            l.fillerPillars.Add(pillar);
        }
    }

    void GenerateOuterWalls(float halfCell, Vector3 halfScale)
    {
        l.wall.transform.localScale = new Vector3(l.cellSize, l.cellSize * wallYScale, wallThickness);

        Vector3 rot;

        // All Nodes with less than 4 edges are around the edges
        List<Graph.Node> edgeNodes = l.Graph.Nodes.FindAll(v => v.Edges.Count < 4);

        foreach (Graph.Node node in edgeNodes)
        {
            // The generation of X ascending and Z ascending walls must be done in slightly different ways
            if (node.CellCenter.x - halfCell == -(halfScale.x) || node.CellCenter.x + halfCell == (halfScale.x))
            {
                rot = new Vector3(0, 90f, 0);
                GenerateXWall();

                // Corners require an additional wall
                if (node.Edges.Count == 2)
                {
                    rot = Vector3.zero;
                    GenerateZWall();
                }

            }
            else
            {
                rot = Vector3.zero;
                GenerateZWall();
            }

            // Nested method for instantiating a wall on the X axis
            void GenerateXWall()
            {
                // Uses a ternary conditional operator to determine which X side the wall should be on
                wallObj = Object.Instantiate(l.wall, node.CellCenter.x - halfCell == -(halfScale.x) ?
                                                    new Vector3(-(halfScale.x), halfCell, node.CellCenter.z) :
                                                    new Vector3(halfScale.x, halfCell, node.CellCenter.z),
                                            Quaternion.Euler(rot));
                wallObj.transform.SetParent(labyrinth.transform);
                l.OuterWall.Add(wallObj);
            }

            // Nested method for instantiating a wall on the Z axis
            void GenerateZWall()
            {
                // Uses a ternary conditional operator to determine which Z side the wall should be on
                wallObj = Object.Instantiate(l.wall, node.CellCenter.z - halfCell == -(halfScale.z) ?
                                                    new Vector3(node.CellCenter.x, halfCell, -(halfScale.z)) :
                                                    new Vector3(node.CellCenter.x, halfCell, halfScale.z),
                                            Quaternion.Euler(rot));
                wallObj.transform.SetParent(labyrinth.transform);
                l.OuterWall.Add(wallObj);
            }
        }
    }
}
