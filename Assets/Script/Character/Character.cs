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

    public virtual void Initialize()
    {
        movableComponent = new CharacterMovementComponent();
        movableComponent.Initialize(characterData);
    }

    public abstract void Update();

    public void HandleInput()
    {
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