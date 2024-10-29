using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerCharacter : Character
{
    public override void Start()
    {
        base.Start();

        ILiveComponent liveComponent = new CharacterLiveComponent();
    }

    public override void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movementVector = new Vector3(moveHorizontal, 0.0f, moveVertical);

        movableComponent.Move(movementVector);
        movableComponent.Rotation(movementVector);
    }
}