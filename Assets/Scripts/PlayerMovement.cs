using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rigidBody;

    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    private Vector2 moveDirection;

    public float moveSpeed;

    public InputActionReference move;

//variables for the jumping  function 
    public InputActionReference jump; 
    public float jumpForce = 5f;
    public float groundCheckDistance = 1.1f;
    private bool jumpRequested;
    private bool isGrounded;
// max jump
    private int jumpsUsed = 0;
    private int maxJumps = 2;

    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.12f;
    private float jumpBufferCounter;
    private float coyoteCounter;
//sprinting function 
//variables for the sprinting function 
    public InputActionReference sprint;
    public float sprintMultiplier = 1.8f;
    private bool isSprinting;
//variables for the crouching function 
    public InputActionReference crouch;
    public CapsuleCollider capsule;
    public float crouchHeight = 1f;
    public float crouchSpeedMultiplier = 0.5f;
    private float standingHeight; 
    private Vector3 standingCenter;
    private bool crouchHeld;
    private bool isCrouching;
//variables for the camera position function
    public Transform cameraPosition;
    public float standingCameraY = 0.25f;
    public float crouchingCameraY = -0.1f;
    public float cameraCrouchSpeed = 10f;
//variables for the water resistance function
    public float waterResistance = 0.5f;
    public float deceleration = 6f;
    public float acceleration = 10f;
    public float maxFallSpeed = 4f;
    public float waterDrag = 2f;
    public float gravityScale = 1f;
    public bool isInWater; 
    public float waterGravityScale = 0.2f;
    public float waterSpeedMultiplier = 0.6f;
    
    private void UpdateCameraHeight()
    {
        Vector3 pos = cameraPosition.localPosition; 
        float targetY = isCrouching ? crouchingCameraY : standingCameraY;
        pos.y = Mathf.Lerp(pos.y, targetY, cameraCrouchSpeed * Time.deltaTime);
        cameraPosition.localPosition = pos;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;// first person mouse 
        if (capsule == null) capsule = GetComponent<CapsuleCollider>();
        standingHeight = capsule.height;
        standingCenter = capsule.center;
        crouchHeight = Mathf.Max(crouchHeight, capsule.radius * 2f);
        rigidBody.useGravity = false;//scaled version 
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime; 
        //look up and down 
        xRotation -= mouseY;
        xRotation = Math.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation,0f,0f);

        //look left and right 
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Update()
    {
        HandleMouseLook();

        moveDirection = move.action.ReadValue<Vector2>();

        crouchHeld = crouch != null && crouch.action.IsPressed();

        isSprinting = sprint != null && sprint.action.IsPressed();
        if (jump != null && jump.action.WasPressedThisFrame())
        {
            Debug.Log("grounded: " + isGrounded);
            jumpRequested = true;
        }
        UpdateCameraHeight();
// release cursor when press escape 
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void FixedUpdate()
    {
        HandleCrouch();
        //check ground 
        Vector3 groundOrigin = new Vector3(
            capsule.bounds.center.x,
            capsule.bounds.min.y + 0.02f,
            capsule.bounds.center.z
        );
        isGrounded = Physics.Raycast (
            groundOrigin,
            Vector3.down,
            0.12f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );
// reset when grounded
        if (isGrounded && rigidBody.velocity.y <= 0f)
        {
            jumpsUsed = 0;
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        if (jumpRequested) jumpBufferCounter = jumpBufferTime; else jumpBufferCounter -= Time.fixedDeltaTime;
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        forward.y = 0f; // so u dont fall in the floor
        right.y = 0f;

        Vector3 direction = Vector3.ClampMagnitude( forward * moveDirection.y + right * moveDirection.x, 1f );

        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        float currentAcceleration = acceleration;
        float currentDeceleration = deceleration;
        float currentGravityScale = gravityScale;

        if(isCrouching) 
            currentSpeed *= crouchSpeedMultiplier;
        if (isInWater)
        {
            currentSpeed *= waterSpeedMultiplier;
            currentAcceleration *= waterResistance;
            currentDeceleration *= waterResistance;
            currentGravityScale = waterGravityScale;
        }
        Vector3 targetVelocity = direction * currentSpeed;
        Vector3 flatVelocity = new(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        float rate = direction.sqrMagnitude > 0.01f ? currentAcceleration : currentDeceleration;
        if(!isGrounded) rate *= 0.5f;
        flatVelocity = Vector3.MoveTowards(flatVelocity, targetVelocity, rate * Time.fixedDeltaTime);


        rigidBody.AddForce(Physics.gravity * currentGravityScale, ForceMode.Acceleration);
        float verticalSpeed = Mathf.Max (rigidBody.velocity.y, -maxFallSpeed);
        
        rigidBody.velocity = new Vector3(flatVelocity.x, verticalSpeed, flatVelocity.z);
    //jump max = 2 
        if (jumpBufferCounter > 0f && jumpsUsed < maxJumps)
        {
            bool canFirstJump = jumpsUsed == 0 && coyoteCounter > 0f;
            bool canSecondJump = jumpsUsed == 1 && !isGrounded;
            if (canFirstJump || canSecondJump)
            {
                rigidBody.velocity = new Vector3(
                    rigidBody.velocity.x,
                    jumpForce,
                    rigidBody.velocity.z
                );
                jumpsUsed++;
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                Debug.Log("jumps used:" + jumpsUsed);
            }
        }
        jumpRequested = false;
    }

    private void HandleCrouch()
    {
        bool wantCrouch = crouchHeld;

        if(isCrouching && !wantCrouch && !CanStandUp())
            wantCrouch = true;

        if (wantCrouch == isCrouching) return;
        isCrouching = wantCrouch;

        float height = isCrouching ? crouchHeight : standingHeight; 
    //feet stay on ground 
        float standingBottom = standingCenter.y - standingHeight /2f;
        capsule.height = height;
        capsule.center = new Vector3(standingCenter.x, standingBottom + height / 2f, standingCenter.z);
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
        GUI.Label(new Rect(10, 30, 400, 20), "Crouch held: " + crouchHeld + " | Crouching: " + isCrouching);
        GUI.Label(new Rect(10, 50, 400, 20), "Capsule height: " + capsule.height);

    }
}
    
