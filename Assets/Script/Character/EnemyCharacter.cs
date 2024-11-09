using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private AiState currentState = AiState.MoveToTarget;
    //[SerializeField] private Character targetCharacter;
    [SerializeField] private float attackRange = 2.0f;

    private float timeBetweenAttackCounter = 0;

    public override Character CharacterTarget => GameManager.Instance.CharacterFactory.GetCharacter(CharacterType.Player);

    public override void Initialize()
    {
        base.Initialize();
        liveComponent = new CharacterLiveComponent();
        liveComponent.Initialize(this);
        damageComponent = new CharacterDamageComponent();
        inputHandler = new AIInputHandler();
    }

    public override void Update()
    {
        switch (currentState)
        {
            case AiState.None:
                break;

            case AiState.MoveToTarget:
                MoveTowardsTarget();

                if (DistanceToTarget() <= attackRange)
                {
                    currentState = AiState.Attack;
                }
                break;

            case AiState.Attack:
                if (DistanceToTarget() <= attackRange)
                {
                    PerformAttack();
                }
                else
                {
                    currentState = AiState.MoveToTarget;
                }
                break;
        }

        if (timeBetweenAttackCounter > 0)
            timeBetweenAttackCounter -= Time.deltaTime;
    }

    private float DistanceToTarget()
    {
        if (CharacterTarget != null)
        {
            return Vector3.Distance(transform.position, CharacterTarget.transform.position);
        }
        Debug.LogError("EnemyCharacter: targetCharacter is not assigned.");
        return float.MaxValue;
    }

    private void MoveTowardsTarget()
    {
        if (CharacterTarget == null || movableComponent == null) return;

        Vector3 direction = (CharacterTarget.transform.position - transform.position).normalized;
        movableComponent.Move(direction);
        movableComponent.Rotation(direction);
    }

    public override void PerformAttack()
    {
        if (timeBetweenAttackCounter <= 0)
        {
            if (damageComponent != null && CharacterTarget != null)
            {
                damageComponent.MakeDamage(CharacterTarget);
                timeBetweenAttackCounter = CharacterData.TimeBetweenAttacks; // Используем свойство CharacterData
            }
            else
            {
                Debug.LogError("EnemyCharacter: damageComponent or targetCharacter is null.");
            }
        }
    }
}
