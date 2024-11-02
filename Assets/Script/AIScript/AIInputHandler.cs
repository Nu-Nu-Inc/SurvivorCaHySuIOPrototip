using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputHandler : IInputHandler
{
    private Vector3 movementDirection;
    private bool attack;

    public Vector3 GetMovementInput()
    {
        return movementDirection;
    }

    public bool IsAttackButtonPressed()
    {
        return attack;
    }

    public void SetMovementDirection(Vector3 direction)
    {
        movementDirection = direction;
    }

    public void SetAttack(bool isAttacking)
    {
        attack = isAttacking;
    }
}
