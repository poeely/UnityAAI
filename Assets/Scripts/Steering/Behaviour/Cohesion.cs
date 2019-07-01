using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Behaviour
{
    private static Cohesion instance;
    public static Cohesion GetInstance()
    {
        if (instance == null)
            instance = new Cohesion();
        return instance;
    }

    public static Vector3 Calculate(Steering instance, List<Agent> entities)
    {
        /*int NeighborCount = 0;
        Vector3 CenterOfMass = Vector3.zero;

        foreach (MovingEntity o in entities)
        {
            if (o != instance)
            {
                CenterOfMass += o.transform.position;

                NeighborCount++;
            }
        }

        if (NeighborCount > 0)
        {
            CenterOfMass /= NeighborCount;

            CenterOfMass = Seek.Calculate(instance, instance.GetUsefulTarget(CenterOfMass));
        }*/

        float neighbourDist = 50f;
        Vector3 sum = new Vector3();
        int count = 0;

        for (int i = 0; i < entities.Count; i++)
        {
            float dist = Vector3.Distance(instance.Owner.Position, entities[i].Position);
            if(dist > 0)
            {
                sum += entities[i].Position;
                count++;
            }
        }

        if (count > 0)
        {
            sum /= (float)count;
            return Seek.Calculate(instance, sum);
        }
        else
            return Vector3.zero;

        //return CenterOfMass;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prCohesion;
        weight = settings.weightCohesion;

        if (/*Random.Range(0.0f, 1.0f) > pr*/true)
        {
            return Calculate(instance, instance.NearEntities) * weight / pr;
        }

        return Vector3.zero;
    }
}
