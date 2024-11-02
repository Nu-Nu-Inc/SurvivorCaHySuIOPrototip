using UnityEngine;

public class PlayerInputHandler : IInputHandler
{
    public Vector3 GetMovementInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        return new Vector3(moveHorizontal, 0.0f, moveVertical);
    }

    public bool IsAttackButtonPressed()
    {
        return Input.GetButtonDown("Fire1");
    }
}
