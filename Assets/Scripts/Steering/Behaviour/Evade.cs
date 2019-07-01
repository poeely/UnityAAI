using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Behaviour
{
    private static Evade instance;
    public static Evade GetInstance()
    {
        if (instance == null)
            instance = new Evade();
        return instance;
    }

    public static Vector3 Calculate(Steering instance, Vector3 target, float panicDist)
    {
        Vector3 ToPersuer = target - instance.transform.position;
        
        float LookAheadTime = ToPersuer.magnitude / (instance.Owner.maxSpeed + target.magnitude);

        return Flee.Calculate(instance, target + (target.normalized * LookAheadTime), panicDist);
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prEvade;
        weight = settings.weightEvade;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            if (instance.TargetTrans != null)
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.TargetTrans.position), settings.panicDist) * weight / pr;
            }
            else
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.TargetPos), settings.panicDist) * weight / pr;
            }
        }

        return Vector3.zero;
    }
}
