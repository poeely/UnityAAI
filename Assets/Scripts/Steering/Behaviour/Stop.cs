using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : Behaviour {

    private static Stop instance;
    public static Stop GetInstance()
    {
        if (instance == null)
            instance = new Stop();
        return instance;
    }

    private float stopLerp = 0.1f;

    public static Vector3 Calculate(Vector3 Velocity, float stopLerp)
    {
        if (Velocity.magnitude < 0.1f)
        {
            return -Velocity;
        }
        return -Vector3.Lerp(Velocity, Vector3.zero, stopLerp);
    }

    public override Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        pr = settings.prStop;
        weight = settings.weightStop;
        stopLerp = settings.lerp;

        if (Random.Range(0.0f, 1.0f) > pr)
        {
            Vector3 velocity = Calculate(instance.Owner.GetVelocity(), stopLerp) * weight / pr;

            if(velocity.magnitude < 0.1f)
                instance.StopOff();

            return velocity;
        }

        return Vector3.zero;
    }
}
