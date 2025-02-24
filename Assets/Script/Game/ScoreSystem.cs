using UnityEngine;

public class ScoreSystem
{
    private const string SAVE_NAME = "MaxScore";
    public int Score { get; private set; }
    public int MaxScore { get; private set; }

    public void StartGame()
    {
        Score = 0;
        MaxScore = PlayerPrefs.GetInt(SAVE_NAME, 0);
        Debug.Log($"Game started. Initial Score: {Score}, MaxScore: {MaxScore}");
    }

    public void EndGame()
    {
        if (Score > MaxScore)
        {
            MaxScore = Score;
            PlayerPrefs.SetInt(SAVE_NAME, MaxScore);
            Debug.Log($"New MaxScore achieved: {MaxScore}");
        }
    }

    public void AddScore(int earnedScore)
    {
        Score += earnedScore;
        Debug.Log($"Added {earnedScore} points. New total: {Score}");
    }

    public void ResetScore()
    {
        Score = 0;
        Debug.Log("Score reset to 0");
    }
}