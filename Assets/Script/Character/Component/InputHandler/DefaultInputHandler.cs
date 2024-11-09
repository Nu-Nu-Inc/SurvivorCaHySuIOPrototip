using UnityEngine;

public class DefaultInputHandler : IInputHandler
{
    public Vector3 GetMovementInput()
    {
        return Vector3.zero; // ���������� ������� ������ ��� ����������� ���������
    }

    public bool IsAttackButtonPressed()
    {
        return false; // ���������� false, ��� ��� ����������� ���������� �� �������� �����
    }
}
