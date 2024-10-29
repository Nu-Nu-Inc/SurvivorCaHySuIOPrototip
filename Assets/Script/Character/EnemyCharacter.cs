using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyCharacter : Character
{
    [SerializeField] private AiState currentState = AiState.MoveToTarget;

    [SerializeField] private Character targetCharacter;

    public override void Start()
    {
        base.Start();

        liveComponent = new ImmortalLiveComponent();
        damageComponent = new CharacterDamageComponent();
    }
    public override void Update()
    {
        switch (currentState)
        {
            case AiState.None:
                // Логика для состояния None
                break;

            case AiState.MoveToTarget:
                if (targetCharacter != null)
                {
                    Vector3 direction = targetCharacter.transform.position - transform.position;
                    direction.y = 0f; // Игнорируем высоту
                    direction.Normalize();

                    movableComponent.Move(direction);
                    movableComponent.Rotation(direction);
                }

                else
                {
                    Debug.LogWarning("Target character is not assigned.");
                }

                if (Vector3.Distance(targetCharacter.transform.position, transform.position) < 3)
                    damageComponent.MakeDamage(targetCharacter);
                break;

            default:
                break;
        }
    }
}