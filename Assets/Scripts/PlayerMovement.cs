using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rigidBody;

    public Transform playerCamera;

    private Vector2 moveDirection;

    public float moveSpeed;

    public InputActionReference move;

    public InputActionReference jump; //jumping  function 
    public float jumpForce = 5f;
    public float groundCheckDistance = 1.1f;
    private bool jumpRequested;
    private bool isGrounded;

    public InputActionReference sprint;
    public float sprintMultiplier = 1.8f;
    private bool isSprinting;

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        isSprinting = sprint != null && sprint.action.IsPressed();
        if (jump.action.WasPressedThisFrame())
        {
            Debug.Log("grounded: " + isGrounded);
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        forward.y = 0f; // so u dont fall in the floor
        right.y = 0f;

        Vector3 direction = forward * moveDirection.y + right * moveDirection.x;

        float currrentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        rigidBody.velocity = new Vector3(direction.x * currrentSpeed, rigidBody.velocity.y, direction.z * currrentSpeed);
        if (jumpRequested && isGrounded)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x,jumpForce, rigidBody.velocity.z);
        }
            jumpRequested = false;
        }
    
    }