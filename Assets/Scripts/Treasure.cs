using UnityEngine;

// Put on a treasure object with a trigger collider.
public class Treasure : MonoBehaviour
{
    public int points = 100;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;

        GameManager.Instance?.CollectTreasure(points);
        gameObject.SetActive(false);
    }
}
