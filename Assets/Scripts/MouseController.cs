using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float yRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        //Get the value of the Horizontal input axis.

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //Get the value of the Vertical input axis.

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
