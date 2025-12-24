using _02.Scripts.Core;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Config")] 
    [SerializeField] private GameConfig _gameConfig;
    
    [Header("State")]
    [SerializeField] private GameState _currentState = GameState.None;
    
    public GameConfig GameConfig => _gameConfig;
    public GameState CurrentState => _currentState;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
    {
        Application.targetFrameRate = 60;
    }

    public void ChangeState(GameState newState)
    {
        _currentState = newState;
        GameEventBus.RaiseGameStateChanged(newState);
        HandleStateChange(newState);
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.LevelUp:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }
}
