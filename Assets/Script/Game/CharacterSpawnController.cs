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
        if (factory == null)
        {
            Debug.LogError("CharacterFactory is null in Initialize!");
            return;
        }

        if (data == null)
        {
            Debug.LogError("GameData is null in Initialize!");
            return;
        }

        characterFactory = factory;
        gameData = data;
        timeBetweenEnemySpawn = gameData.TimeBetweenEnemySpawn;
        spawnTimer = timeBetweenEnemySpawn;
        
        Debug.Log("CharacterSpawnController initialized successfully");
    }

    public void UpdateSpawn(float deltaTime)
    {
        if (characterFactory == null || gameData == null) return;

        timeElapsed += deltaTime;
        spawnTimer -= deltaTime;

        if (timeElapsed >= 60f)
        {
            maxActiveEnemies++;
            timeElapsed = 0f;
            Debug.Log($"Increased max active enemies to: {maxActiveEnemies}");
        }

        // Очищаем список от неактивных врагов
        activeEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeSelf);

        if (spawnTimer <= 0 && activeEnemies.Count < maxActiveEnemies)
        {
            SpawnEnemy();
            spawnTimer = timeBetweenEnemySpawn;
        }
    }

    private void SpawnEnemy()
    {
        if (characterFactory == null)
        {
            Debug.LogError("CharacterFactory is null in SpawnEnemy!");
            return;
        }

        Character enemy = characterFactory.GetCharacter(CharacterType.DefaultEnemy);
        if (enemy == null)
        {
            Debug.LogError("Failed to get enemy from factory!");
            return;
        }

        // Проверяем что у врага есть все необходимые компоненты
        if (enemy.CharacterData == null)
        {
            Debug.LogError($"Enemy {enemy.name} does not have CharacterData assigned!");
            return;
        }

        Vector3 playerPosition = Vector3.zero;
        if (characterFactory.Player != null)
        {
            playerPosition = characterFactory.Player.transform.position;
        }

        float spawnRadius = Random.Range(gameData.MinSpawnOffset, gameData.MaxSpawnOffset);
        float angle = Random.Range(0f, 360f);
        Vector3 spawnOffset = new Vector3(
            spawnRadius * Mathf.Cos(angle * Mathf.Deg2Rad),
            0,
            spawnRadius * Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        enemy.transform.position = playerPosition + spawnOffset;
        enemy.gameObject.SetActive(true);
        enemy.Initialize();

        if (enemy.liveComponent == null)
        {
            Debug.LogError($"Enemy {enemy.name} does not have liveComponent!");
            return;
        }

        // Безопасно подписываемся на событие
        enemy.liveComponent.OnCharacterDeath += OnEnemyDeath;
        activeEnemies.Add(enemy);

        Debug.Log($"Enemy spawned at {enemy.transform.position} with radius {spawnRadius} and angle {angle}");
    }

    private void OnEnemyDeath(Character deadEnemy)
    {
        if (deadEnemy == null) return;

        if (deadEnemy.liveComponent != null)
        {
            deadEnemy.liveComponent.OnCharacterDeath -= OnEnemyDeath;
        }

        deadEnemy.gameObject.SetActive(false);
        activeEnemies.Remove(deadEnemy);
    }

    private void OnDestroy()
    {
        // Очищаем все подписки при уничтожении контроллера
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && enemy.liveComponent != null)
            {
                enemy.liveComponent.OnCharacterDeath -= OnEnemyDeath;
            }
        }
        activeEnemies.Clear();
    }
}