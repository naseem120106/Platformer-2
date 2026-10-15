using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public float sensX;
    public float sensY;

    public Transform orientation;

    float rotationX;
    float rotationY;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        float mouseXinput = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseYinput = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        rotationY += mouseXinput;
        rotationX -= mouseYinput;

        rotationX -= Mathf.Clamp(rotationX, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);

    }
}
