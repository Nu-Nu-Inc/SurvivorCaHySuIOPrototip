using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public override void Start()
    {
        base.Start();

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
