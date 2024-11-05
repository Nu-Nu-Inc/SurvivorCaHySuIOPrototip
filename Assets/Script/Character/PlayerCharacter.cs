using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public override void Initialize()
    {
        base.Initialize();

        liveComponent = new CharacterLiveComponent();

        inputHandler = new PlayerInputHandler();
    }

    public override void Update()
    {
        HandleInput();
    }

    public override void PerformAttack()
    {
        Debug.Log("Player performed an attack.");
    }
}
