using UnityEngine;

public class DefaultInputHandler : IInputHandler
{
    public Vector3 GetMovementInput()
    {
        return Vector3.zero; // Возвращает нулевой вектор как стандартное поведение
    }

    public bool IsAttackButtonPressed()
    {
        return false; // Возвращает false, так как стандартный обработчик не включает атаку
    }
}
