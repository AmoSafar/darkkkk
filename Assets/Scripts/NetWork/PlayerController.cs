using Unity.Netcode;
using UnityEngine;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    void Update()
    {
        if(Input.GetKey(KeyCode.D)){
            transform.Translate(Vector2.right);
        } else if(Input.GetKey(KeyCode.A)){
            transform.Translate(Vector2.left);
        }
        
    }
}