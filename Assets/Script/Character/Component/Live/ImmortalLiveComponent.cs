using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalLiveComponent : ILiveComponent
{
    float ILiveComponent.Health { get => 1; set { } }
    float ILiveComponent.MaxHealth { get => 1; set { } }

    public event Action<Character> OnCharacterDeath;

    public void Initialize(Character selfCharacter)
    {
        //throw new NotImplementedException();
    }

    void ILiveComponent.SetDamage(float damage)
    {
        Debug.Log("ImmortalLiveComponent.SetDamage");
    }
}
