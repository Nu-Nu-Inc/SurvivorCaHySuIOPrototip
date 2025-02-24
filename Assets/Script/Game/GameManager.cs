using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterFactory characterFactory;
    [SerializeField] private GameData gameData;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;

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
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        // Проверка необходимых компонентов в инспекторе
        if (characterFactory == null) Debug.LogError("CharacterFactory not assigned!");
        if (gameData == null) Debug.LogError("GameData not assigned!");
        if (scoreText == null) Debug.LogError("ScoreText not assigned!");
        if (timerText == null) Debug.LogError("TimerText not assigned!");
    }

    private void Initialize()
    {
        Debug.Log("Initializing GameManager...");
        
        scoreSystem = new ScoreSystem();
        spawnController = gameObject.AddComponent<CharacterSpawnController>();
        
        if (characterFactory == null || gameData == null)
        {
            Debug.LogError("Critical components missing in GameManager!");
            return;
        }

        spawnController.Initialize(characterFactory, gameData);
        isGameActive = false;

        UpdateScoreUI(0);
        UpdateTimerUI(gameData.SessionTimeSeconds);
        
        Debug.Log("GameManager initialized successfully");
    }

    public void StartGame()
    {
        Debug.Log("Starting new game...");
        
        gameSessionTime = 0;
        scoreSystem.StartGame();
        UpdateScoreUI(scoreSystem.Score);

        if (isGameActive)
        {
            Debug.Log("Game is already active!");
            return;
        }

        Character player = characterFactory.GetCharacter(CharacterType.Player);
        if (player == null)
        {
            Debug.LogError("Failed to create player!");
            return;
        }

        player.transform.position = Vector3.zero;
        player.gameObject.SetActive(true);

        if (player.liveComponent != null)
        {
            player.liveComponent.OnCharacterDeath -= CharacterDeathHandler;
            player.liveComponent.OnCharacterDeath += CharacterDeathHandler;
            Debug.Log("Player death handler registered");
        }
        else
        {
            Debug.LogError("Player's liveComponent is null during StartGame!");
            return;
        }

        isGameActive = true;
        Debug.Log("Game started successfully");
    }

    private void Update()
    {
        if (!isGameActive) return;

        gameSessionTime += Time.deltaTime;
        UpdateTimerUI(gameData.SessionTimeSeconds - (int)gameSessionTime);

        spawnController.UpdateSpawn(Time.deltaTime);

        if (gameSessionTime >= gameData.SessionTimeSeconds)
        {
            GameVictory();
        }
    }

    private void CharacterDeathHandler(Character deathCharacter)
    {
        if (deathCharacter == null)
        {
            Debug.LogWarning("DeathCharacter is null in death handler!");
            return;
        }

        Debug.Log($"Character death handler triggered for: {deathCharacter.name} of type {deathCharacter.CharacterType}");

        switch (deathCharacter.CharacterType)
        {
            case CharacterType.Player:
                Debug.Log("Player died - Game Over");
                GameOver();
                break;

            case CharacterType.DefaultEnemy:
                if (deathCharacter.CharacterData != null)
                {
                    int scoreCost = deathCharacter.CharacterData.ScoreCost;
                    Debug.Log($"Enemy killed. Adding score: {scoreCost}");
                    scoreSystem.AddScore(scoreCost);
                    UpdateScoreUI(scoreSystem.Score);
                }
                else
                {
                    Debug.LogError($"CharacterData is null for enemy: {deathCharacter.name}");
                }
                break;
        }

        if (deathCharacter.liveComponent != null)
        {
            deathCharacter.liveComponent.OnCharacterDeath -= CharacterDeathHandler;
        }

        deathCharacter.gameObject.SetActive(false);
        characterFactory.ReturnCharacter(deathCharacter);
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
            Debug.Log($"Updated UI score to: {score}");
        }
        else
        {
            Debug.LogError("ScoreText reference is missing!");
        }
    }

    private void UpdateTimerUI(int remainingTime)
    {
        if (timerText == null)
        {
            Debug.LogError("TimerText reference is missing!");
            return;
        }

        int minutes = remainingTime / 60;
        int seconds = remainingTime % 60;
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        isGameActive = false;
        scoreSystem.EndGame();
    }

    private void GameVictory()
    {
        Debug.Log("Victory!");
        isGameActive = false;
        scoreSystem.EndGame();
    }
}