using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : Character
{
    public override Character CharacterTarget
    {
        get
        {
            Character target = null;
            float minDistance = float.MaxValue;
            
            if (GameManager.Instance == null || GameManager.Instance.CharacterFactory == null)
            {
                Debug.LogWarning("GameManager or CharacterFactory is null");
                return null;
            }

            List<Character> list = GameManager.Instance.CharacterFactory.ActiveCharacters;
            if (list == null)
            {
                Debug.LogWarning("ActiveCharacters list is null");
                return null;
            }

            foreach (Character c in list)
            {
                if (c == null || c.CharacterType == CharacterType.Player)
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
        Debug.Log($"Initializing {gameObject.name}");

        // Проверяем наличие CharacterData
        if (CharacterData == null)
        {
            Debug.LogError($"{gameObject.name}: CharacterData is null!");
            return;
        }

        // Инициализируем компонент движения
        movableComponent = new PlayerMovementComponent();
        movableComponent.Initialize(CharacterData);
        if (movableComponent == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to initialize movableComponent!");
            return;
        }

        // Инициализируем компонент жизни
        liveComponent = new CharacterLiveComponent();
        liveComponent.Initialize(this);
        if (liveComponent == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to initialize liveComponent!");
            return;
        }

        // Инициализируем компонент урона
        damageComponent = new CharacterDamageComponent();
        if (damageComponent == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to initialize damageComponent!");
            return;
        }

        // Инициализируем обработчик ввода
        inputHandler = new PlayerInputHandler();
        if (inputHandler == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to initialize inputHandler!");
            return;
        }

        Debug.Log($"{gameObject.name} initialized successfully");
    }

    public override void Update()
    {
        if (!CheckComponents()) return;

        Vector3 movementInput = inputHandler.GetMovementInput();
        
        if (CharacterTarget != null)
        {
            // Поворачиваемся к цели
            Vector3 rotationDirection = (CharacterTarget.transform.position - transform.position).normalized;
            movableComponent.Rotation(rotationDirection);

            // Проверяем атаку
            if (Input.GetButtonDown("Jump") && damageComponent != null)
            {
                damageComponent.MakeDamage(CharacterTarget);
            }
        }
        else if (movementInput != Vector3.zero)
        {
            // Если нет цели, поворачиваемся в направлении движения
            movableComponent.Rotation(movementInput);
        }

        // Двигаемся
        movableComponent.Move(movementInput);
    }

    private bool CheckComponents()
    {
        if (movableComponent == null)
        {
            Debug.LogError($"{gameObject.name}: movableComponent is null! Reinitializing...");
            Initialize();
            return false;
        }

        if (inputHandler == null)
        {
            Debug.LogError($"{gameObject.name}: inputHandler is null! Reinitializing...");
            Initialize();
            return false;
        }

        return true;
    }

    public override void PerformAttack()
    {
        if (CharacterTarget != null && damageComponent != null)
        {
            damageComponent.MakeDamage(CharacterTarget);
            Debug.Log($"{gameObject.name} performed an attack on {CharacterTarget.name}");
        }
    }
}