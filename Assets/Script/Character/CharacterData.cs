using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Settings")]
    [SerializeField] private int scoreCost = 10;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float timeBetweenAttacks = 1f;

    [Header("Prefab References")]
    [SerializeField] private CharacterController characterControllerPrefab;
    [SerializeField] private GameObject characterPrefab; // Изменено с Transform на GameObject

    public int ScoreCost => scoreCost;
    public float Speed { 
        get => speed;
        set => speed = Mathf.Max(0, value); 
    }
    public float TimeBetweenAttacks => timeBetweenAttacks;
    public CharacterController CharacterController => characterControllerPrefab;
    public Transform CharacterTransform => characterPrefab ? characterPrefab.transform : null;

    private void OnValidate()
    {
        speed = Mathf.Max(0, speed);
        timeBetweenAttacks = Mathf.Max(0, timeBetweenAttacks);
        scoreCost = Mathf.Max(0, scoreCost);
    }
}