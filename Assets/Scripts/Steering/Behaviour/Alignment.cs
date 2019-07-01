using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : Behaviour
{
    private static Alignment instance;
    public static Alignment GetInstance()
    {
        if (instance == null)
            instance = new Alignment();
        return instance;
    }

    public static Vector3 Calculate(Steering instance, List<Agent> NearEntities)
    {
        /*Vector3 AverageHeading = Vector3.zero;

        int NeighnorCount = 0;

        foreach (Agent o in NearEntities)
        {
            if (o != instance)
            {
                float dist = Vector3.Distance(instance.Owner.transform.position, o.transform.position);
                AverageHeading += instance.GetUsefulTarget(o.GetVelocity().normalized * (1.0f / dist));

                NeighnorCount++;
            }
        }

        if (NeighnorCount > 0)
        {
            AverageHeading /= NeighnorCount;

            AverageHeading -= instance.Owner.GetVelocity().normalized;
        }*/

        float neighbourDist = 50f;
        Vector3 sum = new Vector3();
        int count = 0;

        for (int i = 0; i < NearEntities.Count; i++)
        {
            float dist = Vector3.Distance(instance.Owner.Position, NearEntities[i].Position);
            if(dist > 0)
            {
                sum += NearEntities[i].GetVelocity();
                count++;
            }
        }

        if (count > 0)
        {
            sum /= (float)count;
            sum.Normalize();
            sum *= instance.Owner.maxSpeed;
            Vector3 steer = sum - instance.Owner.GetVelocity();
            steer = Vector3.ClampMagnitude(steer, instance.Owner.maxForce);
            return steer;
        }
        else
            return Vector3.zero;

        //return AverageHeading;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prAlignment;
        weight = settings.weightAlignment;

        if (/*Random.Range(0.0f, 1.0f) > pr*/true)
        {
            return Calculate(instance, instance.NearEntities) * weight / pr;
        }

        return Vector3.zero;
    }
}
