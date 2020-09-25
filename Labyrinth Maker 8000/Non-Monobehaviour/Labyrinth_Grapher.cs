using UnityEngine;

public class Labyrinth_Grapher
{
    // Safe threshold for StackOverflow Exceptions
    const int MAXIMUM_VERTICES = 8100;

    public Graph GraphFloor(Labyrinth l, int graphSeed)
    {
        Graph graph = new Graph(graphSeed);

        // Check that there won't be too many vertices, and that the walls will be properly aligned
        if (l.graphScale.x * l.graphScale.z / Mathf.Pow(l.cellSize, 2) > MAXIMUM_VERTICES && l.safeMode || 
            l.graphScale.x * l.graphScale.z % l.cellSize != 0 && l.safeMode)
        {
            string errorLog = (l.graphScale.x * l.graphScale.z % l.cellSize != 0) ? 
                    "Please ensure your Graph Scale is divisible by Cell Size with a remainder of zero. Disabling Safe Mode will bypass this check, but will result in geometric anomalies." : 
                    $"Please lower your graph scale or increase cell size so that it creates at most {MAXIMUM_VERTICES} vertices.\nDisabling Safe Mode will bypass this check, but crashes may occur; you are encouraged to make multiple smaller graphs instead.";
            Debug.LogError(errorLog);
            return graph;
        }

        Vector3 cellCenter;

        // Find the center of each cell in the graph, instantiating a new node in the graph to represent each
        for (int i = 0; i < l.graphScale.z / l.cellSize; i++)
        {
            for (int j = 0; j < l.graphScale.x / l.cellSize; j++)
            {
                cellCenter = new Vector3(l.graphScale.x / 2 - j * l.cellSize - l.cellSize / 2, 0, l.graphScale.z / 2 - i * l.cellSize - l.cellSize / 2);
                graph.Nodes.Add(new Graph.Node(cellCenter));
            }
        }

        Graph.Edge edge;

        // For each node created, find its direct neighbours with ascending x or z value. Then make an edge that connects both vertices.
        foreach (Graph.Node node in graph.Nodes)
        {
            foreach (Graph.Node neighbour in graph.Nodes.FindAll(v => v.CellCenter.x == node.CellCenter.x + l.cellSize &&
                                                                      v.CellCenter.z == node.CellCenter.z ||
                                                                      v.CellCenter.z == node.CellCenter.z + l.cellSize &&
                                                                      v.CellCenter.x == node.CellCenter.x))
            {
                edge = new Graph.Edge(node, neighbour);
                node.Edges.Add(edge);
                neighbour.Edges.Add(edge);
            }
        }

        return graph;
    }
}
