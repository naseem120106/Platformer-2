using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class NewBehaviourScript : MonoBehaviour
{
    //Variables that determine the movement of the platform
    public Vector3 moveSpeed = new Vector3(2, 0, 0);
    public Vector3 moveDistance = new Vector3(5, 0, 0);
    public Vector3 originalposition;
    public int switchTimer = 0; //Grace period in frames
    int timer = 0; //Current grace period

    //Calculate the endpoints of the platform's movement
    public Vector3 maxPosition;
    public Vector3 minPosition;



    // Start is called before the first frame update
    void Start()
    {
        originalposition = transform.position;

        //Calculate the endpoints of the platform's movement
        maxPosition = originalposition + moveDistance;
        minPosition = originalposition - moveDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMaxDistanceReached() && timer <= 0){
            moveSpeed = -moveSpeed; // Reverse the direction of movement
            timer = switchTimer; // Reset the grace period 
        }

        transform.position += moveSpeed * Time.deltaTime; // Move the platform
        timer--;
    }

    // This function checks if the platform has reached its maximum distance in any direction
    bool isMaxDistanceReached()
    {
        bool isBoundaryXReached = transform.position.x >= maxPosition.x || transform.position.x <= minPosition.x;
        bool isBoundaryYReached = transform.position.y >= maxPosition.y || transform.position.y <= minPosition.y;
        bool isBoundaryZReached = transform.position.z >= maxPosition.z || transform.position.z <= minPosition.z;

        return isBoundaryXReached || isBoundaryYReached || isBoundaryZReached;
    }
}
