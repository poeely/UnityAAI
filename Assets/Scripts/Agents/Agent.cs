using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Steering))]
public class Agent : MovingEntity
{ 
    private Steering steering;

    protected override void Awake()
    {
        base.Awake();

        steering = GetComponent<Steering>();
        if (steering == null)
            Debug.LogError("No Steering!");
    }

    public void SetDestination(Vector3 targetPosition)
    {
        steering.TargetPos = targetPosition;
        steering.TargetTrans = null;
    }

    public void SetDestination(Transform targetTransform)
    {
        steering.TargetTrans = targetTransform;
    }

    public void Stop()
    {
        if (steering == null)
            return;
        steering.StopOn();
    }

    protected override void FixedUpdate()
    {
        UpdateSteering();
        ApplyVelocity();
    }

    protected virtual void UpdateSteering()
    {
        Vector3 steeringForce = steering.getSteeringForce();
        Vector3 acceleration = steeringForce / rigidbody.mass;

        velocity += acceleration * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }

    protected override void ApplyVelocity()
    {
        if(GetSteering.targetPlane == TargetPlane.XZ)
            velocity.y = 0f;

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity.normalized, transform.up);

        if (GetSteering.targetPlane == TargetPlane.XZ)
            velocity.y = rigidbody.velocity.y;

        rigidbody.velocity = velocity;
    }

    public Steering GetSteering { get { return steering; } }
}
