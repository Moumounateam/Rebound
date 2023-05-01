using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    private float moveSpeed = 25f;

    float horizontalInput = 0;
    float verticalInput = 0;

    void Update()
    {
        if (IsOwner == false) return;
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsOwner == false) return;
        MovePlayerServerRpc(horizontalInput, verticalInput);
    }

    [ServerRpc]
    private void MovePlayerServerRpc(float x, float y) {
        this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(x, 0, y).normalized * moveSpeed);
    }
}
