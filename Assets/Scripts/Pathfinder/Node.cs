using System.Collections.Generic;
using UnityEngine;
/* Author: Vincent J.S. Versnel
 * */
public class Node : IHeapNode
{
    public int x;
    public int z;

    public Vector3 worldPosition;

    public Node parentNode;

    private List<Node> neighbours;

    public float g;
    public float h;

    public float f { get { return g + h; } }

    public bool evaluated;
    public bool walkable;

    public int index { get; set; }
    public int value { get; set; }

    public Node(int x, int z)
    {
        this.x = x;
        this.z = z;

        walkable = true;

        neighbours = new List<Node>();
    }

    public void AddNeighbour(Node neighbour)
    {
        if (neighbour == null || neighbours.Contains(neighbour))
            return;
        neighbours.Add(neighbour);
    }

    public List<Node> GetNeighbours()
    {
        return neighbours;
    }
}
