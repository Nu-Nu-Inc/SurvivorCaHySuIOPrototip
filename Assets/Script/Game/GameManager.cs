using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterFactory characterFactory;
    [SerializeField] private GameData gameData;
    public static GameManager Instance { get; private set; }

    public CharacterFactory CharacterFactory => characterFactory;

    private ScoreSystem scoreSystem;

    private float gameSessionTime;
    private float timeBetweenEnemySpawn;

    private bool isGameActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else Destroy(this.gameObject);
    }

    private void Initialize()
    {
        scoreSystem = new ScoreSystem();
        isGameActive = false;
        //scoreSystem.StartGame();
    }

    public void StartGame()
    {
        gameSessionTime = 0;
        timeBetweenEnemySpawn = gameData.TimeBetweenEnemySpawn;
        //scoreSystem.StartGame();
        if (isGameActive) {
            return;
        }

        Character player = characterFactory.GetCharacter(CharacterType.Player);
        player.transform.position = Vector3.zero;
        player.gameObject.SetActive(true);
        player.Initialize();

        scoreSystem.StartGame();

        isGameActive = true;
    }
    //52 minutes
    private void Update()
    {
        if (isGameActive)
        {
            return;
        }

        gameSessionTime += Time.deltaTime;

        timeBetweenEnemySpawn -= Time.deltaTime;

        if (timeBetweenEnemySpawn < 0)
        {
            SpawnEnemy();
            timeBetweenEnemySpawn = gameData.TimeBetweenEnemySpawn;
        }

        if (gameSessionTime >= gameData.SessionTimeSeconds)
        {
            GameVictory();
        }
    }

    private void CharacterDeathHandler(Character deathCharacter)
    {
        switch (deathCharacter.CharacterType)
        {
            case CharacterType.Player:
                GameOver();
                break;
            case CharacterType.DefaultEnemy:
                scoreSystem.AddScore(deathCharacter.CharacterData.ScoreCost);
                break;
        }
        deathCharacter.gameObject.SetActive(false);
        characterFactory.ReturnCharacter(deathCharacter);
    }

    private void SpawnEnemy()
    {
        Character enemy = characterFactory.GetCharacter(CharacterType.DefaultEnemy);

        Vector3 playerPosition = characterFactory.Player.transform.position;

        enemy.gameObject.SetActive(true);

        float GetOffset()
        {
            bool isPlus = Random.Range(0, 100) % 2 == 0;

            float offset = Random.Range(gameData.MinSpawnOffset, gameData.MaxSpawnOffset);

            return (isPlus) ? offset : (-1 * offset);
        }

        enemy.transform.position = new Vector3(playerPosition.x + GetOffset(), 0, playerPosition.z + GetOffset());
        enemy.Initialize();
    }

    private void GameOver()
    {
        scoreSystem.EndGame();
        Debug.Log("Game Over");
        isGameActive = false;
    }
    private void GameVictory()
    {
        scoreSystem.EndGame();
        Debug.Log("Game Victory");
        isGameActive = false;
    }
}
