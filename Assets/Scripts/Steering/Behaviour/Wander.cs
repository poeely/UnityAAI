using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Behaviour
{
    private static Wander instance;
    public static Wander GetInstance()
    {
        if (instance == null)
            instance = new Wander();
        return instance;
    }

    private static float distance = 1.0f;
    private static float radius = 1.0f;
    private static float jitter = 1.0f;

    public static Vector3 Calculate(Steering instance)
    {
        Vector3 target = new Vector3();

        target += new Vector3(Random.Range(-1f, 1f) * jitter,
                                   Random.Range(-1f, 1f) * jitter,
                                   Random.Range(-1f, 1f) * jitter);

        target = instance.GetUsefulTarget(target);
        target.Normalize();

        target *= radius;


        target += new Vector3(0, 0, distance);

        Vector3 worldTarget = instance.Owner.transform.TransformPoint(target);

        return worldTarget - instance.Owner.transform.position;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prArrive;
        weight = settings.weightArrive;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            return Calculate(instance) * weight / pr;
        }

        return Vector3.zero;
    }
}
