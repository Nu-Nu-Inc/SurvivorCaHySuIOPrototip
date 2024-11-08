using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public override Character CharacterTarget
    {
        get
        {
            Character target = null;
            float minDistance = float.MaxValue;
            List<Character> list = GameManager.Instance.CharacterFactory.ActiveCharacters;
            foreach (Character c in list)
            {
                if (c.CharacterType == CharacterType.Player)
                {
                    continue;
                }

                float distanceBetween = Vector3.Distance(c.transform.position, transform.position);

                if (distanceBetween < minDistance)
                {
                    minDistance = distanceBetween;
                    target = c;
                }
            }
            return target;
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        liveComponent = new CharacterLiveComponent();

        inputHandler = new PlayerInputHandler();
    }

    public override void Update()
    {
        HandleInput();

        float moveHorizontal = inputHandler.GetMovementInput().x;
        float movementVertical = inputHandler.GetMovementInput().z;

        Vector3 movementVector = new Vector3(moveHorizontal, 0, movementVertical).normalized;

        if (CharacterTarget == null)
        {
            movableComponent.Rotation(movementVector);
        }
        else
        {
            Vector3 rotationDirection = (CharacterTarget.transform.position - transform.position).normalized;
            movableComponent.Rotation(rotationDirection);

            if (Input.GetKeyDown("Jump"))
            {
                damageComponent.MakeDamage(CharacterTarget);
            }
        }

        movableComponent.Move(movementVector);
    }

    public override void PerformAttack()
    {
        Debug.Log("Player performed an attack.");
    }
}
