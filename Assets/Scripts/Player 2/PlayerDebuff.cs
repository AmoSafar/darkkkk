using UnityEngine;

public class PlayerDebuff : MonoBehaviour
{
    private SecondPlayerMovement movement;
    private float originalMoveSpeed;
    private float originalJumpForce;
    private bool isDebuffed = false;

    void Start()
    {
        movement = GetComponent<SecondPlayerMovement>();
        if (movement != null)
        {
            originalMoveSpeed = movement.moveSpeed;
            originalJumpForce = movement.jumpForce;
        }
    }

    public void ApplyDebuff(float speedAmount, float jumpAmount, float duration)
    {
        if (movement == null || isDebuffed) return;

        isDebuffed = true;
        movement.moveSpeed *= speedAmount;
        movement.jumpForce *= jumpAmount;

        Invoke(nameof(RemoveDebuff), duration);
    }

    private void RemoveDebuff()
    {
        if (movement == null) return;

        movement.moveSpeed = originalMoveSpeed;
        movement.jumpForce = originalJumpForce;
        isDebuffed = false;
    }
}
