using System;
using UnityEngine;

public class CharacterLiveComponent : ILiveComponent
{
    private Character selfCharacter;

    private float currentHealth;

    public event Action<Character> OnCharacterDeath;

    public float MaxHealth
    {
        get => 50;
        set { /* No implementation needed */ }
    }

    public float Health
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            if (currentHealth > MaxHealth)
                currentHealth = MaxHealth;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                SetDeath();
            }
        }
    }

    public CharacterLiveComponent()
    {
        Health = MaxHealth;
    }

    public void SetDamage(float damage)
    {
        Health -= damage;
        Debug.Log("Get damage = " + damage);
    }

    public void SetDeath()
    {
        if (selfCharacter == null)
        {
            Debug.LogError("selfCharacter is null in CharacterLiveComponent when calling SetDeath");
            OnCharacterDeath?.Invoke(null);
            return;
        }

        OnCharacterDeath?.Invoke(selfCharacter);
        Debug.Log("Death!");
    }

    public void Initialize(Character selfCharacter)
    {
        this.selfCharacter = selfCharacter;
        Debug.Log($"{selfCharacter.gameObject.name} liveComponent initialized with selfCharacter.");
    }
}
