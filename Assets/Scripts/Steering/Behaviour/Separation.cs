using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : Behaviour
{
    private static Separation instance;
    public static Separation GetInstance()
    {
        if (instance == null)
            instance = new Separation();
        return instance;
    }

    public static Vector3 Calculate(Steering instance, List<Agent> entities)
    {
        /*Vector3 average = Vector3.zero;

        foreach (Agent entity in entities)
        {
            if (entity != instance)
            {
                Vector3 toEntitie = instance.Owner.transform.position - entity.transform.position;
                toEntitie = instance.GetUsefulTarget(toEntitie);
                average += toEntitie.normalized / toEntitie.magnitude;
            }
        }*/

        Vector3 steer = new Vector3();
        int count = 0;

        for (int i = 0; i < entities.Count; i++)
        {
            float distance = Vector3.Distance(instance.Owner.Position, entities[i].Position);
            if(distance > 0)
            {
                Vector3 diff = instance.Owner.Position - entities[i].Position;
                diff.Normalize();
                diff /= distance;
                steer += diff;
                count++;
            }
        }

        if (count > 0)
            steer /= (float)count;

        if(steer.magnitude > 0)
        {
            steer.Normalize();
            steer *= instance.Owner.maxSpeed;
            steer -= instance.Owner.GetVelocity();
            steer = Vector3.ClampMagnitude(steer, instance.Owner.maxForce);
        }

        return steer;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prSeparation;
        weight = settings.weightSeparation;

        if (/*Random.Range(0.0f, 1.0f) > pr*/true)
            return Calculate(instance, instance.NearEntities) * weight / pr;
    }
}
