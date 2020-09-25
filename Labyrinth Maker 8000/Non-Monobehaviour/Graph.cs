using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Graph
{
    public List<Node> Nodes { get; private set; } = new List<Node>();

    public int Seed { get; private set; }

    public Graph(int seed)
    {
        Seed = seed;
    }

    /// <summary>
    /// Finds an array of nodes in a rectangular formation of width and length in number of nodes
    /// </summary>
    /// <param name="root">The bottom left node of the rect.</param>
    public Node[] GetRect(Node root, int width, int length, float cellSize)
    {
        Node[] subGraph = new Node[width * length];

        int i = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                subGraph[i] = Nodes.Find(v => v.CellCenter == new Vector3(root.CellCenter.x + (x * cellSize), 
                                                                         root.CellCenter.y, 
                                                                         root.CellCenter.z + (z * cellSize)));
                i++;
            }
        }

        return subGraph;
    }

    public List<Edge> GetRectEdges(Node root, int width, int length, float cellSize)
    {
        List<Edge> edges = new List<Edge>();
        List<Node> nodes = new List<Node>();

        int i = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                 nodes.Add(Nodes.Find(v => v.CellCenter == new Vector3(root.CellCenter.x + (x * cellSize),
                                                                       root.CellCenter.y,
                                                                       root.CellCenter.z + (z * cellSize))));
                i++;
            }
        }

        foreach (Node node in nodes)
        {
            foreach (Edge edge in node.Edges)
            {
                if (nodes.Contains(edge.Node_s) && edge.Node_s != node)
                    edges.Add(edge);
            }
        }

        return edges;
    }

    public Node FindClosestNode(Vector3 position)
    {
        Node closest = Nodes[0];

        for (int i = 1; i < Nodes.Count; i++)
        {
            if (Vector3.Distance(position, Nodes[i].CellCenter) < Vector3.Distance(position, closest.CellCenter))
                closest = Nodes[i];
        }

        return closest;
    }

    public class Node
    {
        public Vector3 CellCenter { get; private set; }
        public Vector3 TileCenter { get; set; } = Vector3.zero;
        public GameObject Floor { get; set; } = null;
        public List<Edge> Edges { get; private set; } = new List<Edge>();

        public Node(Vector3 cellCenter)
        {
            CellCenter = cellCenter;
        }
    }

    public class Edge
    {
        public Node Node_f { get; private set; }
        public Node Node_s { get; private set; }

        public GameObject Wall { get; set; } = null;

        public Edge(Node node_f, Node node_s)
        {
            Node_f = node_f;
            Node_s = node_s;
        }
    }
}
