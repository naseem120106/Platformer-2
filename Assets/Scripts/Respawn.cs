using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Respawn : MonoBehaviour
{
    Vector3 spawnPosition = new Vector3(0, 1, 0); // The position where the player will respawn



    // Respawns the player when they die
    void RespawnPlayer()
    {
        transform.position = spawnPosition;
    }

    // Update is called once per frame; checks the condition for respawning the player
    void Update()
    {
        //if y is -15 then the player should respawn
        if (transform.position.y < -15)
        {
            RespawnPlayer();
        }
    }
}
