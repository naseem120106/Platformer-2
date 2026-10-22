using UnityEngine;

[DisallowMultipleComponent]
public class Respawn : MonoBehaviour
{
    public double deathHeight = -15;
    public Rigidbody rb;

    // The last checkpoint reached; the start position is the default checkpoint
    private Vector3 spawnPosition;

    void Start()
    {
        // Remember where the player starts
        spawnPosition = transform.position;
        // Find the Rigidbody if one isn't assigned
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Called by checkpoints when the player reaches them
    public void SetCheckpoint(Vector3 position)
    {
        spawnPosition = position;
    }

    // Respawns the player at the last checkpoint
    public void RespawnPlayer()
    {
        transform.position = spawnPosition;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    // Update is called once per frame; checks the condition for respawning the player
    void Update()
    {
        if (transform.position.y < deathHeight)
        {
            RespawnPlayer();
        }
    }
}
