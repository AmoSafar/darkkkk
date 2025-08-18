using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    public Camera playerCamera;

    void Start()
    {
        if (!IsOwner)
        {
            playerCamera.enabled = false; // فقط دوربین بازیکن خودت فعال باشه
        }
    }
}
