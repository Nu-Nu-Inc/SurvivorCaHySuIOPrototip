using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;
    [SerializeField] private CharacterData characterData;

    public virtual Character CharacterTarget { get; }
    public CharacterType CharacterType => characterType;
    public CharacterData CharacterData => characterData;

    public IMovable movableComponent { get; protected set; }
    public ILiveComponent liveComponent { get; protected set; }
    public IDamageComponent damageComponent { get; protected set; }

    protected IInputHandler inputHandler;

    protected virtual void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        movableComponent = new CharacterMovementComponent();
        movableComponent.Initialize(characterData);

        liveComponent = new CharacterLiveComponent();
        liveComponent.Initialize(this);

        if (damageComponent == null)
        {
            damageComponent = new CharacterDamageComponent();
            Debug.Log($"{gameObject.name}: damageComponent initialized.");
        }


        // Initialize input handler with a default or base handler
        inputHandler = new DefaultInputHandler();
    }

    public virtual void BaseUpdate()
    {
        HandleInput();
    }

    public abstract void Update();

    public void HandleInput()
    {
        if (inputHandler == null)
        {
            Debug.LogWarning("Input handler is not set for " + gameObject.name);
            return;
        }

        if (movableComponent == null)
        {
            Debug.LogError("Movable component is missing for " + gameObject.name);
            return;
        }

        Vector3 movementInput = inputHandler.GetMovementInput();
        if (movementInput != Vector3.zero)
        {
            movableComponent.Move(movementInput);
            movableComponent.Rotation(movementInput);
        }

        if (inputHandler.IsAttackButtonPressed())
        {
            PerformAttack();
        }
    }

    public abstract void PerformAttack();
}
