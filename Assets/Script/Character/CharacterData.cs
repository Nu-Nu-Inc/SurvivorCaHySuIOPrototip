using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterTransform;

    public float DefaultSpeed => speed;
    public float TimeBetweenAttacks => timeBetweenAttacks;
    public CharacterController CharacterController => characterController;
    public Transform CharacterTransform => characterTransform;
}
