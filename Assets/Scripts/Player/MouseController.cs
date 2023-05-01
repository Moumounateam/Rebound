using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MouseController : NetworkBehaviour
{
    public float mouseSensitivity = 130f;

    [SerializeField]
    public Transform playerBody;

    public override void OnNetworkSpawn() //When the player joins the server
    {
        Debug.Log("Joined");
        print("nique ta mère");
        //find the main camera gameobject
        GameObject camera = Camera.main.gameObject;
        camera.transform.SetParent(transform);
        //Maybe set the right position as well
        //camera.transform.localPosition = [SomeVector3];
    }

    float yRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the value of the Horizontal input axis.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        //Get the value of the Vertical input axis.
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        SendNewRotationServerRpc(yRotation, mouseX);
    }
    
    [ServerRpc]
    private void SendNewRotationServerRpc(float rotation, float mouseX)
    {
        transform.localRotation = Quaternion.Euler(rotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
