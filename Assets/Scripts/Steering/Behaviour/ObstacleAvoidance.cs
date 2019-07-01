using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : Behaviour
{
    private static ObstacleAvoidance instance;
    public static ObstacleAvoidance GetInstance()
    {
        if (instance == null)
            instance = new ObstacleAvoidance();
        return instance;
    }

    public static Vector3 Calculate(Steering instance)
    {
        float dist = 3.0f;
        float shortestDist = float.MaxValue;
        Vector3[] vectors = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0.5f, 0, 0.5f), new Vector3(-0.5f, 0, 0.5f) };
        Vector3 offset = instance.settings.Offset;
        Vector3 bestDir = new Vector3();
        Vector3 bestNormal = new Vector3();

        
        for(int i = 0; i < vectors.Length; i++)
        {
            RaycastHit tempHit;
            Vector3 dir = instance.transform.TransformDirection(vectors[i]);
            if (Physics.Raycast(instance.transform.position + offset, dir, out tempHit, dist, instance.settings.obstacleLayer))
            {
                if (tempHit.distance < shortestDist)
                {
                    bestNormal = tempHit.normal;
                    bestDir = dir;
                    shortestDist = tempHit.distance;
                }
            }
        }

        float penetratedDistance = dist - shortestDist;
        Vector3 direction = Vector3.Reflect(bestDir, bestNormal);

        return direction * instance.Owner.maxSpeed;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prObstacleAvoidence;
        weight = settings.weightOvstacleAvoidence;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            return Calculate(instance) * weight / pr;
        }

        return Vector3.zero;
    }
}
