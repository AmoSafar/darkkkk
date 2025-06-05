using UnityEngine;
// روی بازیکن
public class PlayerPortalBlocker : MonoBehaviour
{
    public bool canTeleport = true;
    public float portalCooldown = 0.2f;

    public void StartPortalCooldown()
    {
        canTeleport = false;
        Invoke(nameof(EnableTeleport), portalCooldown);
    }

    void EnableTeleport()
    {
        canTeleport = true;
    }
}

