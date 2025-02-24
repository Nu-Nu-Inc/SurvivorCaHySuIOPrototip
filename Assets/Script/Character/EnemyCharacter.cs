using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private AiState currentState = AiState.MoveToTarget;
    [SerializeField] private float attackRange = 2.0f;

    private float timeBetweenAttackCounter = 0;

    public override Character CharacterTarget
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.CharacterFactory == null)
            {
                Debug.LogWarning("GameManager or CharacterFactory is null");
                return null;
            }
            return GameManager.Instance.CharacterFactory.GetCharacter(CharacterType.Player);
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        // Проверяем, не проинициализированы ли уже компоненты
        if (liveComponent == null)
        {
            liveComponent = new CharacterLiveComponent();
            liveComponent.Initialize(this);
        }

        if (damageComponent == null)
        {
            damageComponent = new CharacterDamageComponent();
        }

        if (inputHandler == null)
        {
            inputHandler = new AIInputHandler();
        }

        // Сбрасываем состояние
        currentState = AiState.MoveToTarget;
        timeBetweenAttackCounter = 0;
    }

    public override void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        // Проверяем наличие цели
        if (CharacterTarget == null)
        {
            return;
        }

        switch (currentState)
        {
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
        {
            timeBetweenAttackCounter -= Time.deltaTime;
        }
    }

    private float DistanceToTarget()
    {
        if (CharacterTarget == null) return float.MaxValue;
        return Vector3.Distance(transform.position, CharacterTarget.transform.position);
    }

    private void MoveTowardsTarget()
    {
        if (CharacterTarget == null || movableComponent == null || !gameObject.activeInHierarchy) 
        {
            return;
        }

        Vector3 direction = (CharacterTarget.transform.position - transform.position).normalized;
        
        // Только если объект активен
        if (gameObject.activeInHierarchy)
        {
            movableComponent.Move(direction);
            movableComponent.Rotation(direction);
        }
    }

    public override void PerformAttack()
    {
        if (!gameObject.activeInHierarchy) return;

        if (timeBetweenAttackCounter <= 0)
        {
            if (damageComponent != null && CharacterTarget != null && CharacterData != null)
            {
                damageComponent.MakeDamage(CharacterTarget);
                timeBetweenAttackCounter = CharacterData.TimeBetweenAttacks;
            }
        }
    }
}