using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;
    [SerializeField] private CharacterData characterDataAsset;

    public virtual Character CharacterTarget { get; }
    public CharacterType CharacterType => characterType;
    public CharacterData CharacterData => characterDataAsset;

    public IMovable movableComponent { get; protected set; }
    public ILiveComponent liveComponent { get; protected set; }
    public IDamageComponent damageComponent { get; protected set; }

    protected IInputHandler inputHandler;

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void OnValidate()
    {
        if (characterDataAsset == null)
        {
            Debug.LogError($"{gameObject.name}: CharacterData asset is not assigned!");
        }
    }

    public virtual void Initialize()
    {
        Debug.Log($"Initializing character: {gameObject.name}");

        if (characterDataAsset == null)
        {
            Debug.LogError($"{gameObject.name}: CharacterData asset is not assigned!");
            return;
        }

        movableComponent = new CharacterMovementComponent();
        movableComponent.Initialize(characterDataAsset);
        if (movableComponent == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to initialize movableComponent!");
        }

        liveComponent = new CharacterLiveComponent();
        liveComponent.Initialize(this);
        if (liveComponent == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to initialize liveComponent!");
        }

        if (damageComponent == null)
        {
            damageComponent = new CharacterDamageComponent();
            Debug.Log($"{gameObject.name}: damageComponent initialized.");
        }

        inputHandler = new DefaultInputHandler();
        if (inputHandler == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to initialize inputHandler!");
        }

        Debug.Log($"Character {gameObject.name} initialized successfully");
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
            Debug.LogWarning($"Input handler is not set for {gameObject.name}");
            return;
        }

        if (movableComponent == null)
        {
            Debug.LogError($"Movable component is missing for {gameObject.name}");
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

    protected virtual void OnDestroy()
    {
        // Проверяем существование GameManager
        GameManager manager = GameManager.Instance;
        if (manager != null)
        {
            // Безопасно отписываемся от событий
            if (liveComponent != null && liveComponent is CharacterLiveComponent charLive)
            {
                var handler = manager.GetComponent<GameManager>();
                if (handler != null)
                {
                    charLive.RemoveAllListeners();
                }
            }
        }
    }
}