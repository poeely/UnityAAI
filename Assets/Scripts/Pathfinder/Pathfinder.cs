using System;
using System.Threading;
using System.Collections.Generic;

using System.Diagnostics;

using UnityEngine;

/* Author: Vincent J.S. Versnel
 * */

public class PathFinder
{
    // Set the travel cost to 0,0,0 and you've basically transformed A* into GFS
    public static readonly Vector3 DEFAULT_TRAVEL_COST = new Vector3(14, 24, 10);

    public Vector3 travelCost = DEFAULT_TRAVEL_COST;

    public Path SearchAStar(NavGraph graph, Vector3 source, Vector3 target)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        BinaryHeap<Node> open = new BinaryHeap<Node>(graph.Width * graph.Depth);
        HashSet<Node> closed = new HashSet<Node>();

        Node start = graph.GetNode(source);
        if (!start.walkable)
            start = graph.FindNearestWalkable(start);
        Node goal = graph.GetNode(target);
        if (!goal.walkable)
            goal = graph.FindNearestWalkable(goal);

        if (start == null || goal == null)
            return Path.Empty;

        // F cost is used to prioritize
        open.Insert(start, (int)start.f);

        while(open.Count > 0)
        {
            Node current = open.Extract();

            if (current == goal)
            {
                sw.Stop();
                UnityEngine.Debug.Log("[A*]: Found path! Time: " + sw.ElapsedMilliseconds + "ms");
                return ReconstructPath(start, goal);
            }

            closed.Add(current);

            // Evaluate neighbours
            foreach (Node neighbour in current.GetNeighbours())
            {
                if (closed.Contains(neighbour))
                    continue;

                // Calculate cost from source to the neighbour
                float tentative_g = current.g + CalculateHeuristic(current, neighbour);
                
                // If neighbour is already in open, and its not a better path, it does not need to be saved
                if (open.Contains(neighbour) && tentative_g >= neighbour.g)
                    continue;

                // Save this neighbours, as its a better path
                neighbour.parentNode = current;
                neighbour.g = tentative_g;
                neighbour.h = CalculateHeuristic(neighbour, goal);

                if (!open.Contains(neighbour))
                    open.Insert(neighbour, (int)neighbour.f);
                // Because f is now changed, it should be resorted in the heap.
                else
                    open.HeapifyUp(neighbour);
            }
        }
        UnityEngine.Debug.LogError("[A*]: Couldnt find a path");
        return Path.Empty;
    }

    public float CalculateHeuristic(Node a, Node b)
    {
        Vector3 aPos = a.worldPosition, bPos = b.worldPosition;

        aPos.x = aPos.x * travelCost.x;
        aPos.y = aPos.y * travelCost.y;
        aPos.z = aPos.z * travelCost.z;

        bPos.x = bPos.x * travelCost.x;
        bPos.y = bPos.y * travelCost.y;
        bPos.z = bPos.z * travelCost.z;

        return Vector3.Distance(aPos, bPos);
    }

    private Path ReconstructPath(Node start, Node goal)
    {
        List<Vector3> path = new List<Vector3>();
        Node current = goal;

        while(current != start)
        {
            path.Add(current.worldPosition);
            current = current.parentNode;
        }

        path.Reverse();

        return new Path(path);
    }
}

