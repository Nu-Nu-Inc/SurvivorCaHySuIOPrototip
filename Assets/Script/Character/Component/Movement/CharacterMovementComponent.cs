using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterMovementComponent : IMovable
{
    private CharacterData characterData;
    private float speed;
    public float Speed
    {
        get => speed;
        set => speed = value < 0 ? speed : value;

    }

    public void Initialize(CharacterData characterData)
    {
        this.characterData = characterData;
        speed = characterData.DefaultSpeed;
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        characterData.CharacterController.Move(move * speed * Time.deltaTime);
    }

    public void Rotation(Vector3 direction)
    {
        if(direction == Vector3.zero) return;

        float smooth = 0.1f;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(characterData.transform.eulerAngles.y, targetAngle, ref smooth, smooth);
    }

    public void Stop()
    {

    }
}