﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Target Mode
    public Transform target;

    public Vector3 targetOffset;
    public Vector3 cameraOffset;

    public float lerp;

    private float scrollInput;

    // FlyCam
    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private bool mode;
    private void FixedUpdate()
    {
        if(!mode)
            TargetMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            mode = !mode;
        if (mode)
            FlyCam();
    }

    private void TargetMode()
    {
        if (target == null)
            return;

        scrollInput += Input.GetAxis("Mouse ScrollWheel") * 5f;
        scrollInput = Mathf.Clamp(scrollInput, -5f, 30f);

        Vector3 cameraOffsetWorld = target.TransformVector(cameraOffset + Vector3.forward * scrollInput);
        transform.position = Vector3.Lerp(transform.position, target.position + cameraOffsetWorld, lerp);

        Vector3 targetOffsetWorld = target.TransformVector(targetOffset);
        Vector3 direction = (target.position + targetOffsetWorld) - transform.position;
        direction.Normalize();

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void FlyCam()
    {
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Screen.lockCursor = (Screen.lockCursor == false) ? true : false;
        }
    }
}