using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
 
public class CameraController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (base.IsOwner)
        {
            Camera cam = GetComponent<Camera>();
            cam.enabled = true;
        }
    }
}