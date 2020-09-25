using System.Collections.Generic;
using UnityEngine;

public class TileReplacer : MonoBehaviour
{
    const float PLANE_DESCALE = 10f;

    [Tooltip("Holds tile codices for tile replacement. Populate in-editor.")]
    [SerializeField]
    List<CodexEntry> tileCodices = default;

    [Tooltip("Special optional tile that should be 1x1 with no edge buffer and 0 occurences. It will replace the exit tile(s). To disable, set Tile Prefab to none or null.")]
    [SerializeField]
    SO_Tile_Codex.Labyrinth_Tile exitTile = new SO_Tile_Codex.Labyrinth_Tile();

    System.Random r;

    List<Graph.Node> replacedNodes;

    public void ReplaceTiles(Labyrinth l, float wallZScale, int seed = 0)
    {
        // Cache nodes that are replaced so they can not be replaced again
        replacedNodes = new List<Graph.Node>();

        // Start by replacing exit tiles so they are guaranteed to be at the exits
        if (exitTile.TilePrefab != null)
        {
            for (int i = 0; i < l.ExitNodes.Count; i++)
            {
                replacedNodes.Add(l.ExitNodes[i]);
                ReplaceExit(l.ExitNodes[i], l.cellSize, wallZScale, l.floorIsPlane, l.LabyrinthObj.transform);
            }
        }

        // If seed is not 0, use it, otherwise use system time per System.Random default
        if (seed != 0)
            r = new System.Random(Mathf.Abs(seed));
        else
            r = new System.Random();

        // Ensure the floor is generated as tiles
        if (!l.generateFloorAsTiles)
        {
            Debug.LogWarning("Floor was not generated as tiles, cannot run TileReplacer. Returning.");
            return;
        }

        // Iterate through all non-excluded CodexEntries
        foreach (CodexEntry codex in tileCodices.FindAll(entry => !entry.ExcludeCodex))
        {
            // Iterate through all tiles in the CodexEntry
            foreach (SO_Tile_Codex.Labyrinth_Tile tile in codex.TileCodex.Tiles)
            {
                // Instantiate each tile the specified number of times
                for (int i = 0; i < tile.Occurences; i++)
                {
                    // Find nodes that have not been replaced yet and are far enough from the edge of the labyrinth
                    List<Graph.Node> suitableNodes = l.Graph.Nodes.FindAll(v => !replacedNodes.Contains(v) &&
                                                   (v.CellCenter.x <= (l.graphScale.x - l.cellSize) / 2 - (tile.EdgeBuffer + tile.Width - 1) * l.cellSize &&
                                                    v.CellCenter.z <= (l.graphScale.z - l.cellSize) / 2 - (tile.EdgeBuffer + tile.Length - 1) * l.cellSize) &&
                                                   (v.CellCenter.x >= -(l.graphScale.x - l.cellSize) / 2 + tile.EdgeBuffer * l.cellSize &&
                                                    v.CellCenter.z >= -(l.graphScale.z - l.cellSize) / 2 + tile.EdgeBuffer * l.cellSize));

                    if (suitableNodes.Count == 0)
                        goto NodesToReplaceWasNull;

                    // Find a suitable group of nodes to be replaced with the tile
                    Graph.Node[] nodesToReplace = FindSuitableNodeGroup(suitableNodes[r.Next(0, suitableNodes.Count)], tile.Width, tile.Length, l.cellSize, l.Graph, 0);

                    // Skip tile instantiation if nodesToReplace is null
                    if (nodesToReplace == null)
                        goto NodesToReplaceWasNull;

                    // Instantiate the tile with the labyrinth anchor as parent
                    GameObject createdTile = Instantiate(tile.TilePrefab, l.LabyrinthObj.transform);

                    // Destroy the existing floor objects and set the floor to the new tile
                    for (int n = 0; n < nodesToReplace.Length; n++)
                    {
                        if (Application.isEditor)
                            DestroyImmediate(nodesToReplace[n].Floor);
                        else
                            Destroy(nodesToReplace[n].Floor);
                        nodesToReplace[n].Floor = createdTile;
                        replacedNodes.Add(nodesToReplace[n]);
                    }

                    // Scale the tile if autoScale is true
                    if (tile.AutoScale)
                        createdTile.transform.localScale = l.floorIsPlane ? new Vector3(l.cellSize * tile.Width / PLANE_DESCALE, (tile.Width + tile.Length) / 2, l.cellSize * tile.Length / PLANE_DESCALE) :
                                                                            new Vector3(l.cellSize * tile.Width, l.cellSize * wallZScale, l.cellSize * tile.Length);
                    // Center the tile on the cellCenter relative to the labyrinth anchor
                    createdTile.transform.localPosition = FindAndSetNewTileCenters(nodesToReplace);
                    // Ensure the tile is aligned with the parent rotation
                    createdTile.transform.localRotation = Quaternion.identity;

                    // Remove the walls intersecting the tile
                    RemoveIntersectingWalls(nodesToReplace);

                    NodesToReplaceWasNull:;
                }
            }
        }
        // Destroy pillars with no connected walls and remove them from the labyrinth collection
        List<Filler_Pillar> destroyedPillars = new List<Filler_Pillar>();

        foreach (Filler_Pillar pillar in l.fillerPillars)
        {
            if (pillar.ShouldBeDestroyed())
            {
                destroyedPillars.Add(pillar);
            }
        }
        l.fillerPillars.RemoveAll(p => destroyedPillars.Contains(p));
    }

    void ReplaceExit(Graph.Node exit, float cellSize, float wallZScale, bool isPlane, Transform labyrinthAnchor)
    {
        GameObject createdTile = Instantiate(exitTile.TilePrefab, exit.CellCenter, Quaternion.identity, labyrinthAnchor);

        if (exitTile.AutoScale)
            createdTile.transform.localScale = isPlane ? new Vector3(cellSize / PLANE_DESCALE, 1, cellSize / PLANE_DESCALE) :
                                                                new Vector3(cellSize, cellSize * wallZScale, cellSize);

        if (Application.isEditor)
            DestroyImmediate(exit.Floor);
        else
            Destroy(exit.Floor);
    }

    Graph.Node[] FindSuitableNodeGroup(Graph.Node root, int tileWidth, int tileLength, float cellSize, Graph graph, int recursion)
    {
        if (recursion >= 4)
        {
            Debug.LogWarning("Could not place tile within recursive limit, returning null and continuing to next tile.");
            return null;
        }

        Graph.Node[] tileNodes = graph.GetRect(root, tileWidth, tileLength, cellSize);

        for (int i = 0; i < tileNodes.Length; i++)
        {
            if (replacedNodes.Contains(tileNodes[i]))
            {
                tileNodes = FindSuitableNodeGroup(root, tileWidth, tileLength, cellSize, graph, recursion += 1);
                break;
            }
        }

        return tileNodes;
    }

    Vector3 FindAndSetNewTileCenters(Graph.Node[] nodes)
    {
        Vector3 midPoint = Vector3.zero;

        // Sum the values of x and z coordinates
        for (int i = 0; i < nodes.Length; i++)
        {
            midPoint.x += nodes[i].CellCenter.x;
            midPoint.z += nodes[i].CellCenter.z;
        }

        // Divide the x and z sum by number of nodes to find the mid-point. Y should be equal to the other nodes for consistent height.
        midPoint.x /= nodes.Length;
        midPoint.z /= nodes.Length;
        midPoint.y = nodes[0].CellCenter.y;

        // Set the tileCenter of each node to the midPoint
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].TileCenter = midPoint;
        }

        return midPoint;
    }

    void RemoveIntersectingWalls(Graph.Node[] tileNodes)
    {
        List<Graph.Node> tileList = new List<Graph.Node>(tileNodes);

        foreach (Graph.Node node in tileList)
        {
            foreach (Graph.Edge edge in node.Edges)
            {
                if (tileList.Contains(edge.Node_s) && edge.Node_s != node && edge.Wall != null)
                {
                    if (Application.isEditor)
                        DestroyImmediate(edge.Wall);
                    else
                        Destroy(edge.Wall);
                }
            }
        }
    }

    [System.Serializable]
    private class CodexEntry
    {
        [Tooltip("When enabled, excludes this codex from tile replacement.")]
        [SerializeField]
        bool excludeCodex = false;

        [SerializeField]
        SO_Tile_Codex tileCodex = default;

        public bool ExcludeCodex
        {
            get { return excludeCodex; }
        }

        public SO_Tile_Codex TileCodex
        {
            get { return tileCodex; }
        }
    }
}
