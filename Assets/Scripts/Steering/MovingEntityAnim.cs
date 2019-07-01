using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MovingEntityAnim : MovingEntity
{
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        ApplyVelocity();
        //base.FixedUpdate();
    }

    protected override void ApplyVelocity()
    {
        // Rotate towards the velocity direction
        if(velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0f, velocity.z).normalized, Vector3.up);
            
        // Update Animator
        animator.SetFloat("Forward", velocity.magnitude, 0.1f, Time.fixedDeltaTime);
        //animator.SetFloat("Rotation", Mathf.Atan2(velocity.x, velocity.z), 0.1f, Time.fixedDeltaTime);

        //animator.SetBool("IsGrounded", true);
        //animator.SetBool("IsCrouching", false);

        animator.speed = 1f;
    }

    private void OnAnimatorMove()
    {
        if(Time.deltaTime > 0f)
        {
            Vector3 _velocity = (animator.deltaPosition * 1f) / Time.deltaTime;
            _velocity.y = rigidbody.velocity.y;
            rigidbody.velocity = _velocity;
        }
    }
}
