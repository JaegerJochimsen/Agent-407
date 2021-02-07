using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Credit https://www.youtube.com/watch?v=_QajrabyTJc&t=97s

// name of file/script
public class MouseLook : MonoBehaviour
{

    public float mouseSensititvity = 100f;

    public Transform playerBody;

    float xRotation = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // hide and lock cursor to screen center
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        // Get mouse X and Y positions, move them based on sensitivity and lock with framerate 
        // I.e. don't rotate too fast because of framerate
        float mouseX = Input.GetAxis("Mouse X") * mouseSensititvity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensititvity * Time.deltaTime;

        xRotation -= mouseY;
        // clamp rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
