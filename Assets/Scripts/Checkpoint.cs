using UnityEngine;

// Put on an object with a trigger collider. Touching it makes it the player's respawn point.
public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint; // optional; defaults to this object's position
    private bool reached;

    void OnTriggerEnter(Collider other)
    {
        Respawn respawn = other.GetComponentInParent<Respawn>();
        if (respawn == null || reached) return;

        reached = true;
        Vector3 point = respawnPoint != null ? respawnPoint.position : transform.position;
        respawn.SetCheckpoint(point + Vector3.up);
        GameManager.Instance?.ShowMessage("Checkpoint reached");
    }
}
