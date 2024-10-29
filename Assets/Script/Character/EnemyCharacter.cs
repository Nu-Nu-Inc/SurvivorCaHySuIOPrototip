using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private AiState currentState;

    [SerializeField] private Character targetCharacter;

    private float timeBetweenAttackCounter = 0;

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
                break;

            case AiState.MoveToTarget:
                if (targetCharacter != null) // Добавляем проверку
                {
                    Vector3 direction = targetCharacter.transform.position - transform.position;
                    direction.Normalize();

                    movableComponent.Move(direction);
                    movableComponent.Rotation(direction);

                    if (Vector3.Distance(targetCharacter.transform.position, transform.position) < 3
                        && timeBetweenAttackCounter <= 0)
                    {
                        damageComponent.MakeDamage(targetCharacter);
                        timeBetweenAttackCounter = characterData.TimeBetweenAttacks;
                    }

                    if (timeBetweenAttackCounter > 0)
                        timeBetweenAttackCounter -= Time.deltaTime;
                }
                else
                {
                    Debug.LogError("targetCharacter is not assigned in EnemyCharacter.");
                }
                break;
        }
    }

}
