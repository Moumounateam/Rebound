using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CustomNetworkManager : MonoBehaviour
{
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
    
    
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    
    
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}