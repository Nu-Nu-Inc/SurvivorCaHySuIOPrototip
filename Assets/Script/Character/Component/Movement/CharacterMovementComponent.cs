using UnityEngine;

public class CharacterMovementComponent : IMovable
{
    private CharacterData characterData;

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
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        characterData.CharacterController.Move(move * characterData.Speed * Time.deltaTime);
    }

    public void Rotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        float smooth = 0.1f;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(characterData.transform.eulerAngles.y, targetAngle, ref smooth, smooth);

        characterData.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void Stop()
    {
        // Реализация метода остановки (если необходимо)
    }
}
