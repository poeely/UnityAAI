using UnityEngine;

// Author: Vincent J.S. Versnel

/// <summary>
/// Axis-Aligned Bounding Box.
/// </summary>
public class AABB
{
    public Vector3 center;
    public Vector3 size;

    public AABB(Vector3 center, Vector3 size)
    {
        this.center = center;
        this.size = size;
    }

    public bool Contains(Vector3 point)
    {
        // Within width
        return (point.x >= (this.center.x - (size.x / 2)) &&
            point.x <= (this.center.x + (size.x / 2))) &&
        // Within height
            (point.y >= (this.center.y - (size.y / 2)) &&
            point.y <= (this.center.y + (size.y / 2))) &&
        // Within depth
            (point.z >= (this.center.z - (size.z / 2)) &&
            point.z <= (this.center.z + (size.z / 2)));
    }

    public bool Intersects(AABB bb)
    {
        return !(bb.center.x - (bb.size.x / 2) > center.x + (size.x / 2) ||
           bb.center.x + (bb.size.x / 2) < center.x - (size.x / 2) ||
           bb.center.y - (bb.size.y / 2) > center.y + (size.y / 2) ||
           bb.center.y + (bb.size.y / 2) < center.y - (size.y / 2) ||
           bb.center.z - (bb.size.z / 2) > center.z + (size.z / 2) ||
           bb.center.z + (bb.size.z / 2) < center.z - (size.z / 2));
    }

    public void DrawGizmos(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
    }
}
