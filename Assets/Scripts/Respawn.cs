using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Respawn : MonoBehaviour
{
    public Vector3 spawnPosition = new Vector3(0, 1, 0); // The position where the player will respawn

    public double deathHeight = -15;
    public Rigidbody rb;


    void start()
    {
        // Remember where the player starts
        spawnPosition = transform.position;
        // Find the Rigidbody if one isn't assigned
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Respawns the player when they die
    public void RespawnPlayer()
    {
        transform.position = spawnPosition;
    }

    // Update is called once per frame; checks the condition for respawning the player
    void Update()
    {
        //if y is -15 then the player should respawn
        if (transform.position.y < deathHeight)
        {
            RespawnPlayer();
        }
    }
}
