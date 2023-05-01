using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (IsOwner == false || IsServer == true)
        {
            return;
        }
        if (Input.anyKey) {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            MovePlayerServerRpc(horizontalInput, verticalInput);
        }
    }

    [ServerRpc]
    private void MovePlayerServerRpc(float x, float y) {
        // Player Movement
        transform.Translate(new Vector3(x, 0, y) * 30 * Time.deltaTime);
    }
}
