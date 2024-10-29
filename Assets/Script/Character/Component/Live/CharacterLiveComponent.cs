using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLiveComponent : ILiveComponent
{
    private float currentHealth;

    public float MaxHealth
    {
        get => 50;
        set { return; }
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
        Debug.Log("Death!");
    }
}