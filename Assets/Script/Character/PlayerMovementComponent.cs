using UnityEngine;

public class PlayerMovementComponent : IMovable
{
    private CharacterData characterData;
    private float speed;
    private Transform characterTransform;
    private CharacterController characterController;

    public float Speed
    {
        get => speed;
        set => speed = value < 0 ? speed : value;
    }

    public void Initialize(CharacterData characterData)
    {
        this.characterData = characterData;
        if (characterData != null)
        {
            speed = characterData.Speed;
            characterController = characterData.CharacterController;
            characterTransform = characterData.CharacterTransform;
            
            if (characterController == null || characterTransform == null)
            {
                Debug.LogError("CharacterController or CharacterTransform is not assigned in CharacterData!");
            }
        }
        else
        {
            Debug.LogError("CharacterData is null in PlayerMovementComponent Initialize!");
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

        // Трансформация направления относительно камеры
        Vector3 moveDirection = Camera.main.transform.TransformDirection(direction);
        moveDirection.y = 0f;
        moveDirection.Normalize();

        characterController.Move(moveDirection * speed * Time.deltaTime);
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