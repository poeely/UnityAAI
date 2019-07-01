using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : Behaviour
{
    private static Pursuit instance;
    public static Pursuit GetInstance()
    {
        if (instance == null)
            instance = new Pursuit();
        return instance;
    }

    public static Vector3 Calculate(Steering instance, Vector3 target)
    {
        Vector3 toEvader = target - instance.transform.position;

        float RelativeHeading = Vector3.Dot(instance.transform.position.normalized, target.normalized);

        if ((Vector3.Dot(toEvader, instance.transform.position.normalized) > 0) && (RelativeHeading < -0.95))
        {
            return Seek.Calculate(instance, target);
        }

        float LookAheadTime = toEvader.magnitude / instance.Owner.maxSpeed + target.magnitude;

        return Seek.Calculate(instance, target + target.normalized * LookAheadTime);
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prPursuit;
        weight = settings.weightPursuit;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            if (instance.TargetTrans != null)
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.TargetTrans.position)) * weight / pr;
            }
            else
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.TargetPos)) * weight / pr;
            }
        }

        return Vector3.zero;
    }
}
