using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterTransform;

    public float DefaultSpeed => speed;
    public CharacterController CharacterController => characterController;
    public Transform CharacterTransform => characterTransform;
}
