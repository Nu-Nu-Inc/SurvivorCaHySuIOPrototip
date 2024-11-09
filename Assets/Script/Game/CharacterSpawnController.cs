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

        // Начальная установка таймера спавна
        timeBetweenEnemySpawn = gameData.TimeBetweenEnemySpawn;
        spawnTimer = timeBetweenEnemySpawn;
    }

    public void UpdateSpawn(float deltaTime)
    {
        timeElapsed += deltaTime;
        spawnTimer -= deltaTime;

        // Увеличиваем максимальное количество врагов каждую минуту
        if (timeElapsed >= 60f)
        {
            maxActiveEnemies++;
            timeElapsed = 0f; // Сбрасываем таймер для увеличения врагов
        }

        // Спавн врагов, если таймер спавна достиг нуля и количество активных врагов меньше максимального
        if (spawnTimer <= 0 && activeEnemies.Count < maxActiveEnemies)
        {
            SpawnEnemy();
            spawnTimer = timeBetweenEnemySpawn;
        }

        // Удаляем неактивных врагов из списка
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

        // Получаем позицию игрока или используем центр уровня
        Vector3 playerPosition = characterFactory.Player?.transform.position ?? Vector3.zero;

        enemy.gameObject.SetActive(true);

        // Устанавливаем случайную позицию для врага в радиусе вокруг игрока
        float spawnRadius = Random.Range(gameData.MinSpawnOffset, gameData.MaxSpawnOffset);
        float angle = Random.Range(0f, 360f); // случайный угол в градусах

        // Рассчитываем смещение по углу и радиусу
        Vector3 spawnOffset = new Vector3(
            spawnRadius * Mathf.Cos(angle * Mathf.Deg2Rad),
            0, // Уровень земли
            spawnRadius * Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        // Применяем случайную позицию к врагу
        enemy.transform.position = playerPosition + spawnOffset;

        // Лог для проверки позиции спавна
        Debug.Log($"Spawned enemy at position: {enemy.transform.position}, Radius: {spawnRadius}, Angle: {angle}");

        enemy.Initialize();

        if (enemy.liveComponent != null)
        {
            enemy.liveComponent.OnCharacterDeath += OnEnemyDeath;
            activeEnemies.Add(enemy); // Добавляем врага в список активных врагов
        }
        else
        {
            Debug.LogError("Enemy liveComponent is null in SpawnEnemy.");
        }
    }

    private void OnEnemyDeath(Character deadEnemy)
    {
        deadEnemy.gameObject.SetActive(false);
        activeEnemies.Remove(deadEnemy); // Удаляем врага из списка активных
    }
}
