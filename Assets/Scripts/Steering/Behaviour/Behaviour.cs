using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Behaviour
{
    public float pr { get; protected set; }
    protected float weight;



    public virtual Vector3 Calculate(Steering instance, SteeringSettings settings)
    {
        return Vector3.zero;
    }
}
