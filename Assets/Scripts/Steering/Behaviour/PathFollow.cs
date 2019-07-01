using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : Behaviour
{
    private static PathFollow instance;
    public static PathFollow GetInstance()
    {
        if (instance == null)
            instance = new PathFollow();
        return instance;
    }

    public static Vector3 Calculate(Steering instance , Deceleration deceleration, float decelerationRate)
    {
        if (instance.path == null)
            return Vector3.zero;

        instance.path.Update(instance.Owner.Position);

        if (!instance.path.IsFinished)
            return Seek.Calculate(instance, instance.path.Current);
        else
            return Arrive.Calculate(instance, instance.path.Current, deceleration, decelerationRate);
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prPathFollow;
        weight = settings.weightPathFollow;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            
            return Calculate(instance, settings.deceleration, settings.decelerationRate) * weight / pr;
        }

        return Vector3.zero;
    }
}
