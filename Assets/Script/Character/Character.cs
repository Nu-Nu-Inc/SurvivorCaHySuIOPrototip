using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] public CharacterData characterData;
    public IMovable movableComponent { get; protected set; }

    public ILiveComponent liveComponent { get; protected set; }

    public IDamageComponent damageComponent { get; protected set; }


    public virtual void Start()
    {
        movableComponent = new CharacterMovementComponent();
        movableComponent.Initialize(characterData);
    }


    public abstract void Update();
}