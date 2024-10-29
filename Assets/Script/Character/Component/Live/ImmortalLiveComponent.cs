using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalLiveComponent : ILiveComponent
{
    float ILiveComponent.Health { get => 1; set { } }
    float ILiveComponent.MaxHealth { get => 1; set { } }

    void ILiveComponent.SetDamage(float damage)
    {
        Debug.Log("ImmortalLiveComponent.SetDamage");
    }
}
