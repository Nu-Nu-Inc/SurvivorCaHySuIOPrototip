using System;
using UnityEngine;

public class ImmortalLiveComponent : ILiveComponent
{
    private Character selfCharacter;

    public float Health { get => 1; set { } }
    public float MaxHealth { get => 1; set { } }

    // Реализуем событие из интерфейса
    public event Action<Character> OnCharacterDeath;

    public void Initialize(Character selfCharacter)
    {
        this.selfCharacter = selfCharacter;
        Debug.Log($"ImmortalLiveComponent initialized for {selfCharacter.name}");
    }

    public void SetDamage(float damage)
    {
        Debug.Log($"Immortal character {selfCharacter.name} ignored {damage} damage");
        // Бессмертный компонент игнорирует урон, поэтому событие никогда не вызывается
    }
}