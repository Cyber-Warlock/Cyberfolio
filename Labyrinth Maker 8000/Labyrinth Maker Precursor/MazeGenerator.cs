using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    GameObject wall;
    GameObject floor;
    Graph graph = new Graph();
    System.Random r = new System.Random();

    [SerializeField]
    Vector3 floorScale = Vector3.zero;
    [SerializeField]
    float cellSize = 5;
    List<Graph.Cell> visitedcells;
    Stack<Graph.Cell> path;

    // Start is called before the first frame update
    void Start()
    {
        //wall = Resources.Load(@"\Prefabs\Wall") as GameObject;

        floor = GameObject.Instantiate(Resources.Load(@"Prefabs\Floor") as GameObject);
        floor.transform.localScale = new Vector3(floorScale.x, cellSize / 10, floorScale.z);
        wall = Resources.Load(@"Prefabs\Wall") as GameObject;
        graph.GenerateGraph(floorScale, cellSize);
        graph.SetupOuterWalls(floorScale, cellSize, wall, floor);
        Debug.Log(floor.transform.GetChild(0).transform.childCount);
        graph.SetupInnerWalls(floorScale, cellSize, wall, floor);
        DepthFirstSearch_PostGen(graph);

    }

    // Update is called once per frame
    void Update()
    {
        //RunVisit();
    }


    //#region Post-Generation DFS
    public void DepthFirstSearch_PostGen(Graph graph)
    {
        visitedcells = new List<Graph.Cell>();
        path = new Stack<Graph.Cell>();

        Graph.Cell root = graph.cells[r.Next(0, graph.cells.Count)];

        Traverse_PostGen(root);
    }

    //public void Traverse_PostGen(Graph.Cell cell)
    //{

    //    visitedcells.Add(cell);

    //    List<Graph.Edge> unvisistedEdges = cell.Edges.FindAll(v => !visitedcells.Contains(v.Cell1) || !visitedcells.Contains(v.Cell2));

    //    if (unvisistedEdges.Count == 0)
    //    {
    //        Traverse_PostGen(path.Pop());
    //    }
    //    else
    //    {
    //        path.Push(cell);
    //        Shuffle(unvisistedEdges);
    //        foreach (Graph.Edge edge in unvisistedEdges)
    //        {
    //            Destroy(edge.Wall);
    //            Traverse_PostGen(visitedcells.Contains(edge.Cell1) ? edge.Cell2 : edge.Cell1);
    //        }
    //    }
    //}

    public void Traverse_PostGen(Graph.Cell cell)
    {
        visitedcells.Add(cell);

        List<Graph.Edge> unvisistedEdges = cell.Edges.FindAll(v => !visitedcells.Contains(v.Cell1) || !visitedcells.Contains(v.Cell2));

        Shuffle(unvisistedEdges);

        if (unvisistedEdges.Count == 0)
        {
            if (path.Count > 0)
                Traverse_PostGen(path.Pop());
            else
                return;
        }
        else
        {
            path.Push(cell);
            Graph.Edge chosenEdge = unvisistedEdges[r.Next(0, unvisistedEdges.Count)];
            Destroy(chosenEdge.Wall);
            Traverse_PostGen(visitedcells.Contains(chosenEdge.Cell1) ? chosenEdge.Cell2 : chosenEdge.Cell1);
        }
    }

    public void LoopDepth()
    {

    }


    public void Shuffle<T>(List<T> list)
    {
        int i = list.Count;
        while (i >= 1)
        {
            i--;
            int j = r.Next(i + 1);
            T value = list[j];
            Debug.Log(j);
            list[i] = list[j];
            list[i] = value;
        }
    }
}
