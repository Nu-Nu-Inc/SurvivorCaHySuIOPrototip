using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILiveComponent
{
    float Health { get; set; }
    float MaxHealth { get; set; }

    public void SetDamage(float damage);
}
