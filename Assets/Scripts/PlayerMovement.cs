using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rigidBody;

    // The camera the player looks through; PlayerCamera.cs is the only script that rotates it
    public Transform playerCamera;

    private Vector2 moveDirection;

    public float moveSpeed;

    public InputActionReference move;

//variables for the jumping  function 
    public InputActionReference jump; 
    public float jumpForce = 5f;
    public float groundCheckDistance = 1.1f;
    private bool jumpRequested;
    private bool isGrounded;
    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.12f;
    private float jumpBufferCounter;
    private float coyoteCounter;
//sprinting function 
//variables for the sprinting function 
    public InputActionReference sprint;
    public float sprintMultiplier = 1.8f;
    private bool isSprinting;
    private bool sprintHeld;
//variables for the stamina function
    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 25f;
    public float staminaRegenPerSecond = 15f;
    public float staminaRegenDelay = 1f;       // seconds after sprinting before stamina refills
    public float minStaminaToSprint = 20f;     // after running out, stamina must recover this much first
    public float sprintFovIncrease = 10f;      // camera widens while sprinting to show speed
    public float fovChangeSpeed = 8f;
    private float stamina;
    private float staminaRegenTimer;
    private bool exhausted;
    private float sprintVisual;                // 0-1, fades the screen tint in and out
    private Camera playerCam;
    private float baseFov;
    private GUIStyle hudStyle;
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
    
    // Sprinting costs stamina; when it runs out the player has to walk until some has recovered
    private void UpdateSprint()
    {
        bool moving = moveDirection.sqrMagnitude > 0.01f;
        if (exhausted && stamina >= minStaminaToSprint) exhausted = false;

        isSprinting = sprintHeld && moving && !isCrouching && !exhausted;

        if (isSprinting)
        {
            stamina = Mathf.Max(0f, stamina - staminaDrainPerSecond * Time.deltaTime);
            staminaRegenTimer = staminaRegenDelay;
            if (stamina <= 0f)
            {
                exhausted = true;
                isSprinting = false;
            }
        }
        else
        {
            staminaRegenTimer -= Time.deltaTime;
            if (staminaRegenTimer <= 0f)
                stamina = Mathf.Min(maxStamina, stamina + staminaRegenPerSecond * Time.deltaTime);
        }

        // Visual feedback: widen the field of view and fade in a screen tint while sprinting
        sprintVisual = Mathf.MoveTowards(sprintVisual, isSprinting ? 1f : 0f, 6f * Time.deltaTime);
        if (playerCam != null)
        {
            float targetFov = baseFov + (isSprinting ? sprintFovIncrease : 0f);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFov, fovChangeSpeed * Time.deltaTime);
        }
    }

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

        // Movement sets the velocity directly, so ground friction would cancel it and glue the
        // player to the floor. Zero friction (combined with Minimum) lets the script control speed.
        PhysicMaterial noFriction = new PhysicMaterial("PlayerNoFriction");
        noFriction.dynamicFriction = 0f;
        noFriction.staticFriction = 0f;
        noFriction.frictionCombine = PhysicMaterialCombine.Minimum;
        capsule.material = noFriction;
        standingHeight = capsule.height;
        standingCenter = capsule.center;
        crouchHeight = Mathf.Max(crouchHeight, capsule.radius * 2f);
        rigidBody.useGravity = false;//scaled version 
        stamina = maxStamina;
        playerCam = playerCamera.GetComponent<Camera>();
        if (playerCam != null) baseFov = playerCam.fieldOfView;
    }

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        crouchHeld = crouch != null && crouch.action.IsPressed();

        sprintHeld = sprint != null && sprint.action.IsPressed();
        UpdateSprint();
        if (jump != null && jump.action.WasPressedThisFrame())
        {
            Debug.Log("grounded: " + isGrounded);
            jumpRequested = true;
        }
        UpdateCameraHeight();
// release cursor when press escape 
        if(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void FixedUpdate()
    {
        HandleCrouch();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        if (isGrounded) coyoteCounter = coyoteTime; else coyoteCounter -= Time.fixedDeltaTime;
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
        
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.z);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
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

        DrawSprintHud();
    }

    // Stamina bar, "SPRINTING" text and a faint screen tint
    private void DrawSprintHud()
    {
        if (hudStyle == null)
        {
            hudStyle = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        }

        Color previous = GUI.color;

        if (sprintVisual > 0.01f)
        {
            GUI.color = new Color(0.6f, 0.9f, 1f, 0.12f * sprintVisual);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }

        float barWidth = 240f;
        float barHeight = 18f;
        Rect bar = new Rect(Screen.width / 2f - barWidth / 2f, Screen.height - 50f, barWidth, barHeight);
        GUI.color = new Color(0f, 0f, 0f, 0.5f);
        GUI.DrawTexture(bar, Texture2D.whiteTexture);
        GUI.color = exhausted ? new Color(0.9f, 0.25f, 0.25f) : (isSprinting ? new Color(1f, 0.8f, 0.2f) : new Color(0.3f, 0.8f, 1f));
        GUI.DrawTexture(new Rect(bar.x + 2f, bar.y + 2f, (barWidth - 4f) * (stamina / maxStamina), barHeight - 4f), Texture2D.whiteTexture);

        GUI.color = Color.white;
        if (isSprinting)
            GUI.Label(new Rect(0, 60, Screen.width, 40), "SPRINTING", hudStyle);
        else if (exhausted)
            GUI.Label(new Rect(0, 60, Screen.width, 40), "OUT OF STAMINA", hudStyle);

        GUI.color = previous;
    }
}
