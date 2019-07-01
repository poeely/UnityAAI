using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for entities that require movement (Sheep, Cowboy, etc.)
/// 
/// Author: Vincent Versnel
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MovingEntity : Entity
{
    public float maxSpeed = 10.0f;
    public float maxForce = 1f;

    protected Vector3 velocity;
    protected new Rigidbody rigidbody;

    protected override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
            Debug.LogError("No Rigidbody!");
    }

    protected virtual void FixedUpdate()
    {
        ApplyVelocity();
    }
   
    protected virtual void ApplyVelocity()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }

    public Rigidbody GetRigidbody() { return rigidbody; }
    public Vector3 GetVelocity() { return velocity; } 
}
