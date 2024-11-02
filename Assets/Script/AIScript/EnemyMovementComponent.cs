using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementComponent : IMovable
{
    private CharacterData characterData;
    private float speed;
    private float rotationVelocity = 0f;

    public float Speed
    {
        get => speed;
        set => speed = value < 0 ? speed : value;
    }

    public void Initialize(CharacterData characterData)
    {
        this.characterData = characterData;
        speed = characterData.Speed;
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        // Используем направление напрямую
        characterData.CharacterController.Move(direction * speed * Time.deltaTime);
    }

    public void Rotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(
            characterData.CharacterTransform.eulerAngles.y,
            targetAngle,
            ref rotationVelocity,
            0.1f
        );

        characterData.CharacterTransform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void Stop()
    {
        // Реализация остановки при необходимости
    }
}