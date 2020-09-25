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




//public void GenerateLabyrinth()
//{

//}

//public Vector3[] CalculatePlaneCorners(GameObject planeToCalculate)
//{
//    float xScale = (planeToCalculate.transform.localScale.x / 2) * 10;
//    float zScale = (planeToCalculate.transform.localScale.z / 2) * 10;
//    labyrinthCorners[0] = planeToCalculate.transform.TransformPoint(new Vector3(planeToCalculate.transform.position.x + xScale, planeToCalculate.transform.position.y, planeToCalculate.transform.position.z + zScale));
//    labyrinthCorners[1] = planeToCalculate.transform.TransformPoint(new Vector3(planeToCalculate.transform.position.x + xScale, planeToCalculate.transform.position.y, planeToCalculate.transform.position.z - zScale));
//    labyrinthCorners[2] = planeToCalculate.transform.TransformPoint(new Vector3(planeToCalculate.transform.position.x - xScale, planeToCalculate.transform.position.y, planeToCalculate.transform.position.z + zScale));
//    labyrinthCorners[3] = planeToCalculate.transform.TransformPoint(new Vector3(planeToCalculate.transform.position.x - xScale, planeToCalculate.transform.position.y, planeToCalculate.transform.position.z - zScale));
//    return labyrinthCorners;

//}

//public List<GameObject> GenerateCells(Vector3[] corners, GameObject plane, int distanceBetweenCells)
//{
//    List<GameObject> tempList1 = new List<GameObject>();
//    int x = Mathf.CeilToInt(corners[0].x - corners[3].x);
//    int z = Mathf.CeilToInt(corners[0].z - corners[3].z);
//    List<Vector3> tempList = new List<Vector3>();

//    for (int i = 0; i <= x; i += distanceBetweenCells)
//    {
//        for (int j = 0; j <= z; j += distanceBetweenCells)
//        {
//            tempList.Add(new Vector3(corners[0].x - i, corners[0].y, corners[0].z - j));
//        }
//    }
//    for (int i = 0; i < tempList.Count; i++)
//    {
//        cellList.Add(Instantiate(Resources.Load(@"Prefabs\Cell")) as GameObject);
//        cellList[i].transform.localScale = new Vector3(1, 1, 1);
//        cellList[i].transform.parent = plane.transform;
//        cellList[i].transform.position = plane.transform.InverseTransformPoint(tempList[i]);
//    }
//    Debug.Log(cellList.Count);
//    return cellList;
//}

//public void VisitCell(List<GameObject> cellList, Stack<GameObject> cells, int distanceBetweenCells)
//{
//    if (firstTimeVisiting)
//    {
//        currentCell = cellList[Random.Range(0, cellList.Count - 1)];
//    }

//    if (currentCell.GetComponent<Cell>().NeighboursHaveBeenVisited)
//    {
//        currentCell = cellList[Random.Range(0, cellList.Count - 1)];
//    }

//    if (currentCell.GetComponent<Cell>().CellState == CellState.Unvisited)
//    {
//        cells.Push(currentCell);
//        currentCell.GetComponent<Cell>().CellState = CellState.Visited;
//        GameObject neighbourCell = currentCell.GetComponent<Cell>().CheckForNeighbours(distanceBetweenCells);
//        if (neighbourCell != null && neighbourCell.GetComponent<Cell>().CellState == CellState.Unvisited)
//        {
//            currentCell = neighbourCell;
//        }

//    }

//    for (int i = 0; i < cellList.Count; i++)
//    {
//        if (cellList[i].GetComponent<Cell>().CellState == CellState.Unvisited)
//        {
//            break;
//        }
//        else
//        {
//            readyToGenerate = true;
//        }
//    }
//}

//public void RunVisit()
//{
//    if (hasGeneratedLabyrinth == false)
//    {
//        VisitCell(cellList, cellStack, distanceBetweenCells);
//        if (readyToGenerate == true)
//        {
//            GenerateLabyrinth();
//            hasGeneratedLabyrinth = true;
//        }
//    }

//    if (readyToGenerate == true)
//    {
//        Debug.Log(readyToGenerate);
//    }
//}
