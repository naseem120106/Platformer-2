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

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        forward.y = 0f; // so u dont fall in the floor
        right.y = 0f;

        Vector3 direction = forward * moveDirection.y + right * moveDirection.x;

        rigidBody.velocity = new Vector3(direction.x * moveSpeed, rigidBody.velocity.y, direction.z * moveSpeed);
    }
}
