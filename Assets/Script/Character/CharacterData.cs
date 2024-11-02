using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterTransform;

    public float Speed
    {
        get => speed;
        set => speed = value >= 0 ? value : speed;
    }
    public float TimeBetweenAttacks => timeBetweenAttacks;
    public CharacterController CharacterController => characterController;
    public Transform CharacterTransform => characterTransform;
}
