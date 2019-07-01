using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/* Author: Vincent J.S. Versnel
 * */

public class NavGraph
{
    public LayerMask blockingLayer;
    public LayerMask terrainLayer;

    int width, depth;

    Node[,] nodes2d;

    public void Generate(int width, int depth, float heightThreshold)
    {
        this.width = width;
        this.depth = depth;

        nodes2d = new Node[width, depth];

        // Create All Possible Nodes
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float xPos = x + -(width / 2);
                float zPos = z + -(depth / 2);

                Node newNode = new Node(x, z);
                Vector3 pos = new Vector3(xPos, 0f, zPos);

                // Check if position is blocked
                bool blocked = Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, 100f, blockingLayer);
 
                // Check if there is a walkable surface
                RaycastHit hit;
                bool terrain = Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out hit, 100f, terrainLayer);

                newNode.worldPosition = hit.point;
                newNode.walkable = !blocked;

                nodes2d[x, z] = newNode;
            }
        }
        // Assign neighbours to each node
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Depth; j++)
            {
                Node current = nodes2d[i, j];
                if (!current.walkable)
                    continue;

                foreach (Node n in GetNeighbours(current))
                {
                    if (!n.walkable)
                        continue;

                    if (Mathf.Abs(current.worldPosition.y - n.worldPosition.y) > heightThreshold)
                        continue;

                    current.AddNeighbour(n);
                }
            } 
        }
    }

    public void GenerateFloodfill(int width, int depth, float heightThreshold)
    {
        this.width = width;
        this.depth = depth;

        // Initialize grid
        nodes2d = new Node[width, depth];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float xPos = x + -(width / 2);
                float zPos = z + -(depth / 2);

                Node current = new Node(x, z);
                current.worldPosition = new Vector3(xPos, 0f, zPos);

                RaycastHit hit;
                bool terrain = Physics.Raycast(current.worldPosition + Vector3.up * 50f, Vector3.down, out hit, 100f, terrainLayer);
                if (terrain)
                    current.worldPosition = hit.point;
                    
                current.walkable = false;
                nodes2d[x, z] = current;
            }
        }

        // Use floodfill to discover walkability
        BinaryHeap<Node> open = new BinaryHeap<Node>(width * depth);
        HashSet<Node> closed = new HashSet<Node>();
        Node start = GetNode(width / 2, depth / 2);
        open.Insert(start, 0);

        while(open.Count > 0)
        {
            Node current = open.Extract();
            // Check if position is blocked
            bool blocked = Physics.Raycast(current.worldPosition + Vector3.up * 50f, Vector3.down, 100f, blockingLayer);
            current.walkable = !blocked;

            // Check if there is a walkable surface
            closed.Add(current);

            if (current.walkable)
            {
                List<Node> neighbours = GetNeighbours(current);
                foreach (Node n in neighbours)
                {
                    if (closed.Contains(n))
                        continue;
                    if (open.Contains(n))
                        continue;
                    if (Mathf.Abs(current.worldPosition.y - n.worldPosition.y) > heightThreshold)
                        continue;

                    open.Insert(n, (int)Vector3.Distance(start.worldPosition, n.worldPosition));
                }
            }
        }

        Debug.Log("[NAVGRAPH]: Floodfill evaluated: " + closed.Count);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Node current = nodes2d[x, z];
                if (!current.walkable)
                    continue;

                List<Node> neighbours = GetNeighbours(current);
                foreach (Node n in neighbours)
                {
                    if(n.walkable)
                        current.AddNeighbour(n);
                }
            }
        }
    }

    public Node GetNode(int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= depth)
            return null;
        return nodes2d[x, z];
    }

    public Node GetNode(Vector3 position)
    {
        int x = Mathf.Clamp((int)position.x + (width / 2), 0, Width - 1);
        int z = Mathf.Clamp((int)position.z + (depth / 2), 0, Depth - 1);
        return GetNode(x, z);
    }

    public Node FindNearestWalkable(Node errorNode)
    {
        // BFS
        BinaryHeap<Node> open = new BinaryHeap<Node>(Width * Depth);
        HashSet<Node> closed = new HashSet<Node>();

        // G cost is used to prioritize
        open.Insert(errorNode, (int)errorNode.g);

        while (open.Count > 0)
        {
            Node current = open.Extract();

            if (current.walkable)
                return current;

            closed.Add(current);

            foreach (Node n in GetNeighbours(current))
            {
                if (closed.Contains(n))
                    continue;
                if (open.Contains(n))
                    continue;

                open.Insert(n, (int)Vector3.Distance(errorNode.worldPosition, n.worldPosition));
            }
        }
        UnityEngine.Debug.LogError("Could not find nearest node. Nodes evaluated: " + closed.Count);
        return null;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> nodes = new List<Node>();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int xx = node.x + x;
                int zz = node.z + y;

                Node neighbour = GetNode(xx, zz);
                if (neighbour != null)
                    nodes.Add(neighbour);
            }
        }
        return nodes;
    }

    public bool IsObstructed(Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from).normalized;
        float distance = Vector3.Distance(from, to);

        int layermask1 = 1 << blockingLayer;
        int layermask2 = 1 << terrainLayer;
        int finalmask = ~(layermask1 | layermask2); 
        bool obstructed = Physics.Raycast(from + Vector3.up, dir, distance, finalmask);
        return obstructed;
    }

    public bool Generated { get { return nodes2d != null; } }
    public int Width    { get { return width; } }
    public int Depth   { get { return depth; } }
}
