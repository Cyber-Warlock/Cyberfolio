using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Graph
{
    public List<Cell> cells = new List<Cell>();

    public Vector3[] corners = new Vector3[4];

    public void GenerateGraph(Vector3 floorScale, float cellSize)
    {
        for (int i = 0; i < floorScale.x / cellSize; i++)
        {
            for (int j = 0; j < floorScale.z / cellSize; j++)
            {
                cells.Add(new Cell(new Vector3(floorScale.x / 2 - j * cellSize - cellSize / 2, 0, floorScale.z / 2 - i * cellSize - cellSize / 2)));
            }
        }
        foreach (Cell cell in cells)
        {
            foreach (Cell neighbour in cells.FindAll(c => c.Position.x == cell.Position.x + cellSize &&
                                                                    c.Position.z == cell.Position.z ||
                                                                    c.Position.x == cell.Position.x &&
                                                                    c.Position.z == cell.Position.z + cellSize))
            {
                AddEdge(cell, neighbour);

            }
        }
    }


    public void SetupInnerWalls(Vector3 floorScale, float cellSize, GameObject wall, GameObject floor)
    {
        Vector3 wallPosition;
        Vector3 wallRotation;
        GameObject wallObject;

        foreach (Cell cell in cells)
        {
            foreach (Edge edge in cell.Edges)
            {
                if (edge.Wall == null)
                {
                    if (edge.Cell2.Position.x - edge.Cell1.Position.x == cellSize)
                    {
                        wallPosition = new Vector3(edge.Cell2.Position.x - cellSize / 2, cellSize / 2, edge.Cell2.Position.z);
                        wallRotation = new Vector3(0, 90, 0);
                    }
                    else
                    {
                        wallPosition = new Vector3(edge.Cell2.Position.x, cellSize / 2, edge.Cell2.Position.z - cellSize / 2);
                        wallRotation = new Vector3(0, 0, 0);
                    }
                    wallObject = GameObject.Instantiate(wall, wallPosition, Quaternion.Euler(wallRotation));
                    wallObject.transform.localScale = new Vector3(cellSize, cellSize, cellSize * 0.1f);
                    wallObject.transform.SetParent(floor.transform);
                    wallObject.transform.position.Set(wallObject.transform.position.x, floor.transform.position.y, wallObject.transform.position.z);
                    edge.Wall = wallObject;
                }
            }
        }
    }

    public void SetupOuterWalls(Vector3 floorScale, float cellSize, GameObject wall, GameObject floor)
    {
        Vector3 wallPosition;
        Vector3 wallRotation;
        GameObject wallObject;
        GameObject wallParentX = new GameObject();

        wallParentX.transform.parent = floor.transform;


        CalculateFloorCorners(floor);


        float lengthX = corners[0].x - corners[1].x;
        float lengthZ = corners[0].z - corners[1].z;

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < lengthX / cellSize; j++)
            {
                wallPosition = new Vector3((corners[0].x - (cellSize / 2)) - (j * cellSize), cellSize / 2, corners[i].z);
                wallRotation = Vector3.zero;

                wallObject = GameObject.Instantiate(wall, wallPosition, Quaternion.Euler(wallRotation));
                wallObject.transform.localScale = new Vector3(cellSize, cellSize, cellSize * 0.2f);
                wallObject.transform.SetParent(floor.transform.GetChild(0).transform);
                wallObject.transform.position.Set(wallObject.transform.position.x, floor.transform.position.y, wallObject.transform.position.z);
            }
            for (int j = 0; j < lengthZ / cellSize; j++)
            {
                wallPosition = new Vector3(corners[i].x, cellSize / 2, (corners[0].z - (cellSize / 2)) - (j * cellSize));
                wallRotation = new Vector3(0, -90, 0);

                wallObject = GameObject.Instantiate(wall, wallPosition, Quaternion.Euler(wallRotation));
                wallObject.transform.localScale = new Vector3(cellSize, cellSize, cellSize * 0.2f);
                wallObject.transform.SetParent(floor.transform.GetChild(0).transform);
                wallObject.transform.position.Set(wallObject.transform.position.x, floor.transform.position.y, wallObject.transform.position.z);
            }
        }

        GenerateExit(floor, cellSize);
    }

    public void GenerateExit(GameObject floor, float cellSize)
    {
        GameObject wallToSpawnExitAt = floor.transform.GetChild(0).transform.GetChild(Random.Range(0, floor.transform.GetChild(0).transform.childCount - 1)).gameObject;

        GameObject.Destroy(wallToSpawnExitAt);
    }
    public void CalculateFloorCorners(GameObject floor)
    {
        float xScale = floor.transform.localScale.x / 2;
        float zScale = floor.transform.localScale.z / 2;
        //Upper left corner
        corners[0] = new Vector3(floor.transform.position.x + xScale, floor.transform.position.y, floor.transform.position.z + zScale);
        //Lower right corner
        corners[1] = new Vector3(floor.transform.position.x - xScale, floor.transform.position.y, floor.transform.position.z - zScale);
        //Upper right corner
        corners[2] = new Vector3(floor.transform.position.x - xScale, floor.transform.position.y, floor.transform.position.z + zScale);
        //Lower left corner
        corners[3] = new Vector3(floor.transform.position.x + xScale, floor.transform.position.y, floor.transform.position.z - zScale);
    }

    public void AddEdge(Cell a, Cell b)
    {
        Edge tempEdge = new Edge(a, b);
        a.Edges.Add(tempEdge);
        b.Edges.Add(tempEdge);
    }

    public class Cell
    {
        List<Edge> edges = new List<Edge>();
        Vector3 position;

        public List<Edge> Edges { get { return edges; } set { edges = value; } }
        public Vector3 Position { get { return position; } set { position = value; } }

        public Cell(Vector3 position)
        {
            this.Position = position;
        }
    }

    public class Edge
    {
        Cell cell1;
        Cell cell2;
        GameObject wall;

        public Cell Cell1 { get { return cell1; } }
        public Cell Cell2 { get { return cell2; } }

        public GameObject Wall { get { return wall; } set { wall = value; } }

        public Edge(Cell cell1, Cell cell2)
        {
            this.cell1 = cell1;
            this.cell2 = cell2;
        }

    }
}
