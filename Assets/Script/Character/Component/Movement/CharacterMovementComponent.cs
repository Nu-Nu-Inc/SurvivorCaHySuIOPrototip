using UnityEngine;

public class CharacterMovementComponent : IMovable
{
    private CharacterData characterData;
    private Transform characterTransform;
    private CharacterController characterController;

    public float Speed
    {
        get => characterData.Speed;
        set
        {
            if (value >= 0)
            {
                characterData.Speed = value;
            }
        }
    }

    public void Initialize(CharacterData characterData)
    {
        this.characterData = characterData;
        if (characterData != null)
        {
            this.characterController = characterData.CharacterController;
            this.characterTransform = characterData.CharacterTransform;
            if (characterController == null || characterTransform == null)
            {
                Debug.LogError("CharacterController or CharacterTransform is not assigned in CharacterData!");
            }
        }
        else
        {
            Debug.LogError("CharacterData is null in CharacterMovementComponent Initialize!");
        }
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero || characterController == null) return;

        // Проверяем, активен ли GameObject и компонент
        if (!characterController.gameObject.activeInHierarchy || !characterController.enabled)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        characterController.Move(move * characterData.Speed * Time.deltaTime);
    }

    public void Rotation(Vector3 direction)
    {
        if (direction == Vector3.zero || characterTransform == null) return;

        float smooth = 0.1f;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(characterTransform.eulerAngles.y, targetAngle, ref smooth, smooth);

        characterTransform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void Stop()
    {
        // Реализация остановки при необходимости
    }
}