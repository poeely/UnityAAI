using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnim : Agent
{
    protected Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void ApplyVelocity()
    {
        // Rotate towards the velocity direction
        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0f, velocity.z).normalized, Vector3.up);

        //Debug.Log(velocity);
        // Update Animator
        animator.SetFloat("Forward", velocity.magnitude, 0.1f, Time.fixedDeltaTime);
        animator.speed = velocity.magnitude;
    }

    // Only called when method == Method.ANIMATED
    private void OnAnimatorMove()
    {
        if (animator == null)
            return;
        if (Time.deltaTime > 0f)
        {
            Vector3 _velocity = (animator.deltaPosition) / Time.deltaTime;
            _velocity.y = rigidbody.velocity.y;
           
            rigidbody.velocity = _velocity;
        }
    }
}
