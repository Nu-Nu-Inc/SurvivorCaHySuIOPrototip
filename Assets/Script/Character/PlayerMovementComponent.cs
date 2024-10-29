using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : IMovable
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

        // Преобразование направления относительно камеры
        Vector3 moveDirection = Camera.main.transform.TransformDirection(direction);
        moveDirection.y = 0f;
        moveDirection.Normalize();

        characterData.CharacterController.Move(moveDirection * speed * Time.deltaTime);
    }

    public void Rotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        float smooth = 0.1f;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(characterData.transform.eulerAngles.y, targetAngle, ref smooth, smooth);
    }


    public void Stop()
    {
        // Реализация остановки при необходимости
    }
}

