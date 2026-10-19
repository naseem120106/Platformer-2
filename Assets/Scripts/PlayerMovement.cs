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

//jumping  function 
    public InputActionReference jump; 
    public float jumpForce = 5f;
    public float groundCheckDistance = 1.1f;
    private bool jumpRequested;
    private bool isGrounded;
//sprinting function 
    public InputActionReference sprint;
    public float sprintMultiplier = 1.8f;
    private bool isSprinting;
//crouching function 
    public InputActionReference crouch;
    public CapsuleCollider capsule;
    public float crouchHeight = 1f;
    public float crouchSpeedMultiplier = 0.5f;
    private float standingHeight; 
    private Vector3 standingCenter;
    private bool crouchHeld;
    private bool isCrouching;
    
    private void Start()
    {
        if (capsule == null) capsule = GetComponent<CapsuleCollider>();
        standingHeight = capsule.height;
        standingCenter = capsule.center;
    }

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        crouchHeld = crouch != null && crouch.action.IsPressed();

        isSprinting = sprint != null && sprint.action.IsPressed();
        if (jump.action.WasPressedThisFrame())
        {
            Debug.Log("grounded: " + isGrounded);
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        HandleCrouch();
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

    private void HandleCrouch()
    {
        bool wantCrouch = crouchHeld;

        if(isCrouching &&!wantCrouch && !CanStandUp())
            wantCrouch = true;
        if (wantCrouch && isCrouching) return;
        isCrouching = wantCrouch;

        float height = isCrouching ? crouchHeight : standingHeight; 
    //feet stay on ground 
        float standingBottom = standingCenter.y - standingHeight /2f;
        capsule.height = height;
        capsule.center = new Vector3(standingCenter.x, standingBottom + height / 2);
    }
    private bool CanStandUp()
    {
        float r = capsule.radius * 0.95f;
        float standingBottom = standingCenter.y - standingHeight / 2f;
        float crouchedTop = standingBottom + crouchHeight;

        Vector3 origin = transform.position +  new Vector3(standingCenter.x, crouchedTop - capsule.radius, standingCenter.z);
        float distance = standingHeight - crouchHeight; 
        return !Physics.SphereCast(origin, r, Vector3.up, out _, distance, ~0, QueryTriggerInteraction.Ignore);
    }
    private void OnGUI()
    {
        Vector3 flatVelocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        GUI.Label(new Rect(10, 10, 200, 20), "Velocity: " + flatVelocity.magnitude.ToString("F2"));
    }
}
    
