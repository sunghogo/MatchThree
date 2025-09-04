using UnityEngine;
using System;

public enum GameState
{
    StartingScreen,
    GameStart,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnHighScoreChanged;
    public static event Action<int> OnMovesChanged;
    public static event Action OnMatch;

    public static event Action OnNextLevel;
    public static event Action OnGameStart;
    public static event Action OnGameOver;
    public static event Action OnScreenStart;
    public static event Action OnGameWin;

    [field: Header("Game States")]
    [field: SerializeField] public bool StartingScreen { get; private set; }
    [field: SerializeField] public bool GameStart { get; private set; }
    [field: SerializeField] public bool GameOver { get; private set; }
    [field: SerializeField] public bool ChangeLevel { get; private set; }
    [field: SerializeField] public bool GameWin { get; private set; }

    [field: Header("Shared Data")]
    [field: SerializeField] public int Score { get; private set; } = 0;
    [field: SerializeField] public int HighScore { get; private set; } = 0;
    [field: SerializeField] public int Moves { get; private set; } = 10;

    public void IncrementScore()
    {
        ++Score;
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }

    void UpdateHighScore()
    {
        HighScore = Score;
        OnHighScoreChanged?.Invoke(HighScore);
    }

    public void DecrementMoves()
    {
        --Moves;
        OnMovesChanged?.Invoke(Moves);
        if (Moves == 0) EndGame();
    }

    public void ResetMoves()
    {
        Moves = 10;
        OnMovesChanged?.Invoke(Moves);
    }

    public void StartGame()
    {
        StartingScreen = false;
        GameStart = true;
        GameOver = false;
        ChangeLevel = false;
        GameWin = false;
        if (Score > HighScore)
        {
            UpdateHighScore();
        }
        ResetMoves();
        ResetScore();
        OnGameStart?.Invoke();
        OnHighScoreChanged?.Invoke(HighScore);
    }

    public void EndGame()
    {
        StartingScreen = false;
        GameStart = false;
        GameOver = true;
        ChangeLevel = false;
        GameWin = false;
        OnGameOver?.Invoke();
    }

    public void StartScreen()
    {
        StartingScreen = true;
        GameStart = false;
        GameOver = false;
        ChangeLevel = false;
        GameWin = false;
        OnScreenStart?.Invoke();
    }

    public void NextLevel()
    {
        ChangeLevel = true;
        OnNextLevel?.Invoke();
    }

    public void WinGame()
    {
        StartingScreen = false;
        GameStart = false;
        GameOver = false;
        ChangeLevel = false;
        GameWin = true;
        OnGameWin?.Invoke();
    }

    public void Matched()
    {
        OnMatch?.Invoke();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Instance.StartScreen();   
    }
}
