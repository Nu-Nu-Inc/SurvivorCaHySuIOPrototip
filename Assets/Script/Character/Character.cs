using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected CharacterData characterData;
    public IMovable movableComponent { get; protected set; }

    public ILiveComponent liveComponent { get; protected set; }

    public IDamageComponent damageComponent { get; protected set; }


    public virtual void Start()
    {
        if (this is PlayerCharacter)
        {
            movableComponent = new PlayerMovementComponent();
        }
        else if (this is EnemyCharacter)
        {
            movableComponent = new EnemyMovementComponent();
        }
        movableComponent.Initialize(characterData);
    }

    public abstract void Update();
}