using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerPlayerMovement : NetworkBehaviour
{
    void Update()
    {
        if (IsServer == false)
        {
            return;
        }
        SendNewPositionClientRpc(transform.position);
    }

    [ClientRpc]
    private void SendNewPositionClientRpc(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime);
    }
}
