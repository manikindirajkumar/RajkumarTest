using System;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    public class MockGameManager : IGameManager
    {
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;
        public event Action OnGameRestart;

        public bool StartGameCalled   { get; private set; }
        public bool GameOverTriggered { get; private set; }
        public bool RestartCalled     { get; private set; }

        public void StartGame()
        {
            StartGameCalled = true;
            CurrentState    = GameState.Playing;
            OnStateChanged?.Invoke(CurrentState);
        }

        public void TriggerGameOver()
        {
            GameOverTriggered = true;
            CurrentState      = GameState.GameOver;
            OnStateChanged?.Invoke(CurrentState);
        }

        public void RestartGame()
        {
            RestartCalled = true;
            OnGameRestart?.Invoke();
            CurrentState  = GameState.Playing;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}