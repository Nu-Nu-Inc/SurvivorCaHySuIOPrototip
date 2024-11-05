using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterFactory characterFactory;
    public static GameManager Instance { get; private set; }

    public CharacterFactory CharacterFactory => characterFactory;

    private ScoreSystem scoreSystem;

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
        //scoreSystem.StartGame();
        if(isGameActive) {
            return;
        }

        Character player = characterFactory.GetCharacter(CharacterType.Player);
        player.transform.position = Vector3.zero;

        isGameActive = true;
    }
}
