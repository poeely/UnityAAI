using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : Behaviour
{
    private static Seek instance;
    public static Seek GetInstance()
    {
        if (instance == null)
            instance = new Seek();
        return instance;
    }

    public static Vector3 Calculate(Steering instance, Vector3 target)
    {
        Vector3 dir = (target - instance.transform.position);
        dir.Normalize();
        Vector3 desired = dir * instance.Owner.maxSpeed;
        Vector3 actual = (desired);
        return actual;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prSeek;
        weight = settings.weightSeek;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            if(instance.TargetTrans != null)
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
