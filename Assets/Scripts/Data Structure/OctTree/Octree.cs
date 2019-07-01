using System.Collections.Generic;

using UnityEngine;

// Author: Vincent J.S. Versnel

public interface IPoint
{
    Vector3 Position { get; }
}

/// <summary>
/// An Octree used to query objects in a given AABB.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Octree<T> where T : IPoint
{
    public AABB boundary;
    public List<Octree<T>> subtrees;

    private int capacity;
    private List<T> points;
    private bool divided;

    public Octree(AABB boundary, int capacity)
    {
        this.boundary = boundary;
        this.capacity = capacity;

        points = new List<T>();
        subtrees = new List<Octree<T>>();
    }
	
    public bool Insert(T point)
    {
        if (!boundary.Contains(point.Position))
            return false;

        if (points.Count < capacity)
        {
            points.Add(point);
            return true;
        }
        else
        {
            if (!divided)
                SubDivide();

            foreach (var subTree in subtrees)
                if (subTree.Insert(point))
                    return true;
        }

        throw new System.Exception("[OctTree]: Impossible Error.");
    }

    public List<T> Query(AABB range)
    {
        List<T> found = new List<T>();
        if (!boundary.Intersects(range))
            return found;
        else
        {
            for (int i = 0; i < points.Count; i++)
                if (range.Contains(points[i].Position))
                    found.Add(points[i]);

            if(divided)
                foreach (var subtree in subtrees)
                    found.AddRange(subtree.Query(range));

            return found;
        }
    }

    public void Query(AABB range, List<T> list)
    {
        if (!boundary.Intersects(range))
            return;
        else
        {
            for (int i = 0; i < points.Count; i++)
                if (range.Contains(points[i].Position))
                    list.Add(points[i]);

            if (divided)
                foreach (var subtree in subtrees)
                    subtree.Query(range, list);
        }
    }

    public void Clear()
    {
        foreach (var subtree in subtrees)
            subtree.Clear();

        subtrees.Clear();
        points.Clear();
        divided = false;
    }

    private void SubDivide()
    {
        Vector3 newSize = (boundary.size / 2);
        Vector3 offset = newSize / 2;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 point = boundary.center + new Vector3(x * newSize.x, y * newSize.y, z * newSize.z) - offset;
                    AABB bb = new AABB(point, newSize);
                    Octree<T> subtree = new Octree<T>(bb, capacity);
                    subtrees.Add(subtree);
                }  
            }
        }
        divided = true;
    }
}
