using System.Collections.Generic;
using UnityEngine;

public class Labyrinth_DFS
{
    List<Graph.Node> visitedVertices;

    System.Random r;

    public void DepthFirstSearch(Graph graph)
    {
        // If a seed is specified, use that, otherwise instantiate System.Random with no seed
        if (graph.Seed != 0)
            r = new System.Random(Mathf.Abs(graph.Seed));
        else
            r = new System.Random();

        visitedVertices = new List<Graph.Node>();

        // Find a random root node to start. No need to add an edge to self
        Graph.Node root = graph.Nodes[r.Next(0, graph.Nodes.Count)];

        // Start traversal from root node
        Traverse(root);
    }

    void Traverse(Graph.Node node)
    {
        // Visit the node
        visitedVertices.Add(node);

        // Fisher-Yates Shuffle to randomize edge indices, this approach allows easy looping recursion with randomization
        // Can handle almost 5 times as many vertices as a non-looped recursion!
        Shuffle(node.Edges);

        foreach (Graph.Edge edge in node.Edges)
        {
            // If either node on the edge is unvisited, then Traverse to it. Otherwise fall through and return to previous recursive layer
            if (!visitedVertices.Contains(edge.Node_f) || !visitedVertices.Contains(edge.Node_s))
            {
                // When generating at runtime, use Destroy(). When generating in EditMode use DestroyImmediate()
                if (Application.isPlaying)
                {
                    Object.Destroy(edge.Wall);
                    edge.Wall = null;
                }
                else
                {
                    Object.DestroyImmediate(edge.Wall);
                    edge.Wall = null;
                }

                // Ternary Conditional operator to pass the unvisited node
                Traverse(visitedVertices.Contains(edge.Node_s) ? edge.Node_f : edge.Node_s);
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        int i;
        T value;
        while (n > 1)
        {
            n--;
            i = r.Next(n + 1);
            value = list[i];
            list[i] = list[n];
            list[n] = value;
        }
    }

    public void GenerateExits(List<GameObject> outerWalls, Vector3 graphScale, Labyrinth l, bool singleExit = false)
    {
        if (outerWalls.Count < 1)
        {
            Debug.LogWarning("OuterWalls was empty. Returning.");
            return;
        }

        // Find the first exit as a random outer wall
        GameObject exit = outerWalls[r.Next(0, outerWalls.Count)];
        l.ExitNodes.Add(l.Graph.FindClosestNode(exit.transform.position));

        // Find all outer walls on the opposite side
        List<GameObject> opposingWalls = outerWalls.FindAll(wall => Mathf.Abs(wall.transform.position.x * -1 + exit.transform.position.x) == graphScale.x ||
                                                                    Mathf.Abs(wall.transform.position.z * -1 + exit.transform.position.z) == graphScale.z);

        // When generating at runtime, use Destroy(). When generating in EditMode use DestroyImmediate()
        if (Application.isPlaying)
        {
            Object.Destroy(exit);
            // If only a single exit should be created, return now.
            if (singleExit)
                return;

            // Destroy a random wall from the opposing walls as the other exit
            exit = opposingWalls[r.Next(0, opposingWalls.Count)];
            l.ExitNodes.Add(l.Graph.FindClosestNode(exit.transform.position));
            Object.Destroy(exit);
        }
        else
        {
            Object.DestroyImmediate(exit);
            if (singleExit)
                return;

            // Destroy a random wall from the opposing walls as the other exit
            exit = opposingWalls[r.Next(0, opposingWalls.Count)];
            l.ExitNodes.Add(l.Graph.FindClosestNode(exit.transform.position));
            Object.DestroyImmediate(exit);
        }
    }
}
