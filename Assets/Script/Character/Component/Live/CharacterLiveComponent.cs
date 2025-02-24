using System;
using UnityEngine;

public class CharacterLiveComponent : ILiveComponent
{
    private float health;
    private float maxHealth = 100f;
    private Character selfCharacter;

    public event Action<Character> OnCharacterDeath;

    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);
            if (health <= 0)
            {
                Die();
            }
        }
    }

    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value > 0 ? value : maxHealth;
    }

    public void Initialize(Character selfCharacter)
    {
        this.selfCharacter = selfCharacter;
        Health = maxHealth;
        Debug.Log($"{selfCharacter.name} liveComponent initialized with selfCharacter.");
    }

    public void SetDamage(float damage)
    {
        if (damage < 0)
        {
            Debug.LogWarning($"Attempted to set negative damage ({damage}) to {selfCharacter?.name}");
            return;
        }

        Health -= damage;
        Debug.Log($"{selfCharacter?.name} took {damage} damage. Health: {Health}");
    }

    private void Die()
    {
        Debug.Log($"{selfCharacter?.name} died");
        OnCharacterDeath?.Invoke(selfCharacter);
    }

    public void RemoveAllListeners()
    {
        if (OnCharacterDeath != null)
        {
            foreach (Delegate d in OnCharacterDeath.GetInvocationList())
            {
                OnCharacterDeath -= (Action<Character>)d;
            }
        }
    }
}