using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    public static BirdManager instance;

    public Bird birdPrefab;

    public int birdCount;

    public float speed;
    public float alignment;
    public float cohesion;
    public float separation;

    public Vector3 size;

    private AABB bounding;

    private void Awake()
    {
        instance = this;

        bounding = new AABB(transform.position, size);

        for (int i = 0; i < birdCount; i++)
        {
            Vector3 pos = transform.position + new Vector3(size.x * Random.value, size.y * Random.value, size.z * Random.value) - (size/2);
            Bird bird = Instantiate(birdPrefab, pos, Quaternion.identity);
            bird.aabb = bounding;
        }
    }

    private void OnDrawGizmos()
    {
        if(bounding == null)
            bounding = new AABB(transform.position, size);
        bounding.size = size;
        bounding.center = transform.position;
        bounding.DrawGizmos(Color.red);
    }
}
