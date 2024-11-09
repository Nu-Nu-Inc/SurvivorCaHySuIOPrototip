using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawnController : MonoBehaviour
{
    private CharacterFactory characterFactory;
    private GameData gameData;

    private float timeBetweenEnemySpawn;
    private float spawnTimer;
    private int maxActiveEnemies = 5;
    private float timeElapsed;

    private List<Character> activeEnemies = new List<Character>();

    public void Initialize(CharacterFactory factory, GameData data)
    {
        characterFactory = factory;
        gameData = data;

        // ��������� ��������� ������� ������
        timeBetweenEnemySpawn = gameData.TimeBetweenEnemySpawn;
        spawnTimer = timeBetweenEnemySpawn;
    }

    public void UpdateSpawn(float deltaTime)
    {
        timeElapsed += deltaTime;
        spawnTimer -= deltaTime;

        // ����������� ������������ ���������� ������ ������ ������
        if (timeElapsed >= 60f)
        {
            maxActiveEnemies++;
            timeElapsed = 0f; // ���������� ������ ��� ���������� ������
        }

        // ����� ������, ���� ������ ������ � ���������� ������ ������ �������������
        if (spawnTimer <= 0 && activeEnemies.Count < maxActiveEnemies)
        {
            SpawnEnemy();
            spawnTimer = timeBetweenEnemySpawn;
        }

        // ������� ���������� ������ �� ������
        activeEnemies.RemoveAll(enemy => !enemy.gameObject.activeSelf);
    }

    private void SpawnEnemy()
    {
        Character enemy = characterFactory.GetCharacter(CharacterType.DefaultEnemy);

        if (enemy == null)
        {
            Debug.LogError("Enemy is null in SpawnEnemy.");
            return;
        }

        // ������� ������

        Vector3 playerPosition = characterFactory.Player?.transform.position ?? Vector3.zero;

        enemy.gameObject.SetActive(true);

        float spawnRadius = Random.Range(gameData.MinSpawnOffset, gameData.MaxSpawnOffset);
        float angle = Random.Range(0f, 360f); 

        Vector3 spawnOffset = new Vector3(
            spawnRadius * Mathf.Cos(angle * Mathf.Deg2Rad),
            0, // ������� �����
            spawnRadius * Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        enemy.transform.position = playerPosition + spawnOffset;

        Debug.Log($"Spawned enemy position: {enemy.transform.position}, Radius: {spawnRadius}, Angle: {angle}");

        enemy.Initialize();

        if (enemy.liveComponent != null)
        {
            enemy.liveComponent.OnCharacterDeath += OnEnemyDeath;
            activeEnemies.Add(enemy);  
        }
        else
        {
            Debug.LogError("Enemy liveComponent is null in Enemy.");
        }
    }

    private void OnEnemyDeath(Character deadEnemy)
    {
        deadEnemy.gameObject.SetActive(false);
        activeEnemies.Remove(deadEnemy); // ������� ����� �� ������ ��������
    }
}
