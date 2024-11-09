using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterFactory characterFactory;
    [SerializeField] private GameData gameData;
    [SerializeField] private Text scoreText; // UI текст для очков
    [SerializeField] private Text timerText; // UI текст для таймера

    public static GameManager Instance { get; private set; }

    public CharacterFactory CharacterFactory => characterFactory;

    private ScoreSystem scoreSystem;
    private CharacterSpawnController spawnController;

    private float gameSessionTime;
    private bool isGameActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Initialize()
    {
        scoreSystem = new ScoreSystem();
        spawnController = gameObject.AddComponent<CharacterSpawnController>();
        spawnController.Initialize(characterFactory, gameData);

        isGameActive = false;

        UpdateScoreUI(0); // Начальное обновление очков
        UpdateTimerUI(gameData.SessionTimeSeconds); // Начальное обновление таймера
    }

    public void StartGame()
    {
        gameSessionTime = 0;
        scoreSystem.ResetScore(); // Сброс очков перед началом игры
        UpdateScoreUI(scoreSystem.Score); // Обновление UI очков

        if (isGameActive)
            return;

        Character player = characterFactory.GetCharacter(CharacterType.Player);
        player.transform.position = Vector3.zero;
        player.gameObject.SetActive(true);

        if (player.liveComponent != null)
        {
            player.liveComponent.OnCharacterDeath -= CharacterDeathHandler;
            player.liveComponent.OnCharacterDeath += CharacterDeathHandler;
        }
        else
        {
            Debug.LogError("Player's liveComponent is null during StartGame.");
        }

        isGameActive = true;
    }

    private void Update()
    {
        if (isGameActive)
        {
            gameSessionTime += Time.deltaTime;
            UpdateTimerUI(gameData.SessionTimeSeconds - (int)gameSessionTime);

            spawnController.UpdateSpawn(Time.deltaTime);

            if (gameSessionTime >= gameData.SessionTimeSeconds)
            {
                GameVictory();
            }
        }
    }

    private void CharacterDeathHandler(Character deathCharacter)
    {
        if (deathCharacter == null)
            return;

        switch (deathCharacter.CharacterType)
        {
            case CharacterType.Player:
                GameOver();
                break;
            case CharacterType.DefaultEnemy:
                if (deathCharacter.CharacterData != null)
                {
                    scoreSystem.AddScore(deathCharacter.CharacterData.ScoreCost);
                    UpdateScoreUI(scoreSystem.Score); // Обновление очков в UI
                }
                break;
        }

        deathCharacter.gameObject.SetActive(false);
        characterFactory.ReturnCharacter(deathCharacter);

        if (deathCharacter.liveComponent != null)
        {
            deathCharacter.liveComponent.OnCharacterDeath -= CharacterDeathHandler;
        }
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateTimerUI(int remainingTime)
    {
        int minutes = remainingTime / 60;
        int seconds = remainingTime % 60;

        if (timerText != null)
        {
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    private void GameOver()
    {
        isGameActive = false;
    }

    private void GameVictory()
    {
        isGameActive = false;
    }

    public class ScoreSystem
    {
        public int Score { get; private set; }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }
}
