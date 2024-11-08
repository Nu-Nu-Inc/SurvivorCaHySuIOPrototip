using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] private int sessionTimeMinutes = 15;
    [SerializeField] private float timeBetweenEnemySpawn = 1.5f;
    [SerializeField] private float minSoawnOffset = 7;
    [SerializeField] private float maxSpawnOffset = 9;

    public int SessionTimeMinutes => sessionTimeMinutes;
    public int SessionTimeSeconds => sessionTimeMinutes * 60;
    public float TimeBetweenEnemySpawn => timeBetweenEnemySpawn;
    public float MinSpawnOffset => minSoawnOffset;
    public float MaxSpawnOffset => maxSpawnOffset;
}
