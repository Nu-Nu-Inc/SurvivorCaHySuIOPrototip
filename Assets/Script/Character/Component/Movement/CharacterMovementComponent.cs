using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterMovementComponent : IMovable
{
    private float speed;
    public float Speed
    {
        get => speed;
        set => speed = value < 0 ? speed : value;

    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
    }

    public void Rotation(Vector3 direction)
    {

    }

    public void Stop()
    {

    }
}