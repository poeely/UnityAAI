using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<Vector3> points;
    private int currentEdge;
    private bool finished;

    public Path(List<Vector3> points)
    {
        this.points = points;
        finished = false;
    }

    // Chooses the next point
    // Returns false if there is no next point
    public void Update(Vector3 agentPos)
    {
        if (finished)
            return;

        if (Vector3.Distance(Current, agentPos) >= 0.5f)
            return;

        ++currentEdge;
        // Equal to because the PathFollow will know to use arrive instead of seek on the last point.
        if (currentEdge >= (points.Count - 1))
        {
            currentEdge = Mathf.Min(currentEdge, (points.Count - 1));
            if (Vector3.Distance(Current, agentPos) < 0.5f)
                finished = true;
        }
    }

    public Vector3 Current
    {
        get
        {
            return points[currentEdge];
        }
    }

    // * * * * Post Processing * * * * //
    public void Reset()
    {
        currentEdge = 0;
        finished = false;
    }

    public void Simplify()
    {
        Reset();
        List<Vector3> simplifiedPath = new List<Vector3>();
        Vector3 oldDir = Vector3.zero;
        for (int i = 0; i < (points.Count - 1); i++)
        {
            Vector3 a = points[i];
            Vector3 b = points[i + 1];
            a.y = 0f;
            b.y = 0f;

            Vector3 newDir = (b - a).normalized;
            if (newDir != oldDir)
                simplifiedPath.Add(points[i]);
            oldDir = newDir;
        }
        points.Clear();
        points.AddRange(simplifiedPath);
    }

    public void SmoothPrecise()
    {
        Reset();
        // For each point, see if another point is reachable
    }

    public void SmoothQuick()
    {
        Reset();
        // If there is line of sight from i to i + 2, remove i + 1
        int i = 0;
        while(i < (points.Count - 2))
        {
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 2];

            bool obstructed = GameWorld.Instance.NavGraph.IsObstructed(p1, p2);
            if (obstructed)
                i++;
            else
                points.RemoveAt(i + 1);
        }

        Debug.Log("[PATH]: Smoothed, edges remaining: " + points.Count);
    }
	
    // Helper
    public void DrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 from = points[i];
            Vector3 to = points[i + 1];
            Gizmos.DrawLine(from + Vector3.up, to + Vector3.up);
            Gizmos.DrawCube(from, Vector3.one);
            Gizmos.DrawCube(to, Vector3.one);
        }
    }

    public bool IsFinished { get { return finished; } }

    public static Path Empty { get { return new Path(new List<Vector3>()); } }
}
