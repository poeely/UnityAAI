using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : Behaviour
{
    private static Arrive instance;
    public static Arrive GetInstance()
    {
        if (instance == null)
            instance = new Arrive();
        return instance;
    }

    private Deceleration deceleration;
    private float decelerationRate = 0.3f;

    public static Vector3 Calculate(Steering instance, Vector3 target, Deceleration deceleration, float decelerationRate)
    {
        Vector3 ToTarget = target - instance.transform.position;

        float dist = ToTarget.magnitude;

        if (dist > 0.0)
        {
            float speed = dist / ((float)deceleration * decelerationRate);

            //owner.maxSpeed doesnt work
            speed = Mathf.Min(speed, instance.Owner.maxSpeed);

            Vector3 desiredVelocity = (ToTarget * speed) / dist;

            return (desiredVelocity - instance.Owner.GetVelocity());
        }

        return Vector3.zero;
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prArrive;
        weight = settings.weightArrive;
        deceleration = settings.deceleration;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            if (instance.TargetTrans != null)
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.TargetTrans.position), deceleration, decelerationRate) * weight / pr;
            }
            else
            {
                return Calculate(instance, instance.GetUsefulTarget(instance.TargetPos), deceleration, decelerationRate) * weight / pr;
            }
        }

        return Vector3.zero;
    }
}
