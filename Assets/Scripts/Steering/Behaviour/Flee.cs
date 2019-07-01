using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Behaviour
{
    private static Flee instance;
    public static Flee GetInstance()
    {
        if (instance == null)
            instance = new Flee();
        return instance;
    }

    private float panicDist = 10.0f;

    public static Vector3 Calculate(Steering instance, Vector3 target, float panicDist)
    {
        if (Vector3.Distance(instance.transform.position, target) < panicDist)
        {
            Vector3 velocity = (instance.transform.position - target).normalized * instance.Owner.maxSpeed;

            return velocity;
        }
        return Vector3.zero;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prFlee;
        weight = settings.weightFlee;
        panicDist = settings.panicDist;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            if (instance.TargetTrans != null)
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.ThreatTrans.position), panicDist) * weight / pr;
            }
            else
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.ThreatPos), panicDist) * weight / pr;
            }
        }

        return Vector3.zero;
    }
}
